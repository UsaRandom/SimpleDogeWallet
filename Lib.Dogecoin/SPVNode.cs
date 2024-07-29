using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Lib.Dogecoin.Interop;

namespace Lib.Dogecoin
{
    public class SPVNode
    {
        private const int BLOCKS_BETWEEN_CHECKPOINTS = 10;

        private uint _lastSPVCheckpointHeight = 0;

        private bool _syncComplete = false;
        private bool _isDebug;
        private Thread _thread;
        private IntPtr _spvNodeRef;
        private int _peerCount;
        private static dogecoin_spv_client.sync_transaction_delegate syncTransactionCallback;
        private static dogecoin_spv_client.sync_completed_delegate syncCompletedCallback;
        private static dogecoin_spv_client.header_message_processed_delegate headerMessageProcessedCallback;

        private SPVNodeBlockInfo _startPoint;

        public SPVNode(ISPVCheckpointTracker tracker, bool isMainNet, bool isDebug, SPVNodeBlockInfo startPoint, int peerCount = 24)
        {
            CheckpointTracker = tracker;
            _isDebug = isDebug;
            IsMainNet = isMainNet;
            _startPoint = startPoint;
            _peerCount = peerCount;
        }

        public ISPVCheckpointTracker CheckpointTracker { get; set; }

        public bool IsRunning { get; private set; }

        public bool IsMainNet { get; private set; }

        public bool SyncComplete
        {
            get
            {
                if (!_syncComplete)
                {
                    if (_spvNodeRef != IntPtr.Zero && IsRunning)
                    {
                        var spv = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

                        return spv.called_sync_completed;
                    }
                }
                return _syncComplete;
            }
        }

        public SPVNodeBlockInfo CurrentBlockInfo { get; private set; }

        public Action OnSyncComplete { get; set; }

        public bool UseFullSync { get; set; }

        public Action<SPVNodeTransaction> OnTransaction { get; set; }

        public Action<SPVNodeBlockInfo, SPVNodeBlockInfo> OnNextBlock { get; set; }
        public Action<SPVNodeBlockInfo> OnProcessedHeaders { get; set; }

        private void BeforeOnNextBlock(SPVNodeBlockInfo previousBlock, SPVNodeBlockInfo nextBlock)
        {
            if (CheckpointTracker != null)
            {
                CheckpointTracker.SaveCheckpoint(nextBlock);
            }

            if (OnNextBlock != null)
            {
                OnNextBlock(previousBlock, nextBlock);
            }
        }

        public unsafe int GetPeerCount()
        {
            if (_spvNodeRef == IntPtr.Zero)
            {
                return 0;
            }

            var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

            var nodeGroup = Marshal.PtrToStructure<dogecoin_node_group>(client.nodegroup);

            var nodeList = *nodeGroup.nodes;

            var connectedNodes = 0;

            for (var i = 0; i < nodeList.len; i++)
            {
                dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

                if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED)
                {
                    connectedNodes++;
                }
            }

            return connectedNodes;
        }

        public unsafe uint GetBestKnownHeight()
        {

            try
            {
                if(_spvNodeRef == IntPtr.Zero)
                {
                    return 0;
                }

                var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

                var nodeGroup = Marshal.PtrToStructure<dogecoin_node_group>(client.nodegroup);

                var nodeList = *nodeGroup.nodes;

                var heights = new Dictionary<uint, int>();

                var connectedNodes = 0;
                for (var i = 0; i < nodeList.len; i++)
                {
                    dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

                    if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED)
                    {
                        connectedNodes++;

                        if (heights.ContainsKey(node.bestknownheight))
                        {
                            heights[node.bestknownheight] = heights[node.bestknownheight]++;
                        }
                        else
                        {
                            heights.Add(node.bestknownheight, 1);
                        }
                        Debug.WriteLine("Best Known Height: " + node.bestknownheight);
                    }
                }

                return heights.OrderByDescending(a => a.Value).First().Key;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return 0;
        }

        public unsafe void PrintDebug()
        {
            try
            {
                Debug.WriteLine("Thread Status: " + (IsRunning ? "Online" : "Offline"));



                var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

                var nodeGroup = Marshal.PtrToStructure<dogecoin_node_group>(client.nodegroup);

                var nodeList = *nodeGroup.nodes;

                Debug.WriteLine("Nodes: " + nodeList.len);

                var connectedNodes = 0;
                for (var i = 0; i < nodeList.len; i++)
                {
                    dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

                    Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");
                    if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED)
                    {
                        connectedNodes++;

                        //last requested block id thingy


                        Debug.WriteLine("Best Known Height: " + node.bestknownheight);
                        Debug.WriteLine("Last Requested Block: " + LittleEndianByteArrayToHexString(node.last_requested_inv));
                    }
                }

                Debug.WriteLine("Connected Nodes: " + connectedNodes);

                //Debug.WriteLine($"Block.Hash: {this.CurrentBlockInfo.Hash}");
                //Debug.WriteLine($"Block.BlockHeight: {this.CurrentBlockInfo.BlockHeight}");
                //Debug.WriteLine($"Block.Timestamp: {this.CurrentBlockInfo.Timestamp.ToLocalTime()}");

                Debug.WriteLine($"spv.stateflags: {client.stateflags}");
                Debug.WriteLine($"spv.last_statecheck_time: {DateTimeOffset.FromUnixTimeSeconds((long)client.last_statecheck_time).ToLocalTime()}");
                Debug.WriteLine($"spv.last_headersrequest_time: {DateTimeOffset.FromUnixTimeSeconds((long)client.last_headersrequest_time).ToLocalTime()}");

                Debug.WriteLine($"nodeGroup.desired_amount_connected_nodes: {nodeGroup.desired_amount_connected_nodes}");
                Debug.WriteLine($"nodeGroup.clientstr: {nodeGroup.clientstr.TerminateNull()}");


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        public void Start()
        {
            if (_thread != null && _thread.IsAlive)
            {
                throw new Exception("SPV Already Running!");
            }

            CreateSPVClient();

            _thread = new Thread(() =>
            {
                IsRunning = true;
                _syncComplete = false;

                LibDogecoinInterop.dogecoin_spv_client_discover_peers(_spvNodeRef, null);
                var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);


                unsafe
                {
                    var headerDb = *client.headers_db;

                    if (headerDb.has_checkpoint_start(client.headers_db_ctx) == 0)
                    {
                        SPVNodeBlockInfo checkpoint = null;

                        if (_startPoint != null)
                        {
                            checkpoint = _startPoint;
                        }
                        else if (CheckpointTracker != null)
                        {
                            checkpoint = CheckpointTracker.GetCheckpoint();
                        }

                        if (checkpoint != null)
                        {
                            headerDb.set_checkpoint_start(client.headers_db_ctx,
                                                          HexStringToLittleEndianByteArray(checkpoint.Hash),
                                                          checkpoint.BlockHeight);
                        }
                    }

                }


                LibDogecoinInterop.dogecoin_node_group_connect_next_nodes(client.nodegroup);


                LibDogecoinInterop.dogecoin_spv_client_runloop(_spvNodeRef);


                IsRunning = false;
            });

            _thread.Start();
        }

        public unsafe void Stop()
        {
            var spv = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

            LibDogecoinInterop.dogecoin_node_group_shutdown(spv.nodegroup);

         //   _thread.Abort();

            //		LibDogecoinInterop.dogecoin_spv_client_free(_spvNodeRef);


            //var nodeList = *nodeGroup.nodes;

            //Debug.WriteLine("Nodes: " + nodeList.len);

            //var connectedNodes = 0;
            //for (var i = 0; i < nodeList.len; i++)
            //{
            //	dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

            //	Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");
            //	if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED || (NODE_STATE)node.state == NODE_STATE.NODE_CONNECTING)
            //	{
            //		connectedNodes++;
            //	}
            //	LibDogecoinInterop.dogecoin_node_free(*(nodeList.data + i));
            //	Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");

            //}
        }


        ~SPVNode()
        {
            if (_spvNodeRef != IntPtr.Zero)
            {
                LibDogecoinInterop.dogecoin_spv_client_free(_spvNodeRef);
            }
        }


        private unsafe void CreateSPVClient()
        {


            if(_spvNodeRef == IntPtr.Zero)
            {
                LibDogecoinInterop.dogecoin_spv_client_free(_spvNodeRef);
            }

            var net = IsMainNet ? LibDogecoinContext._mainChain : LibDogecoinContext._testChain;

            _spvNodeRef = LibDogecoinInterop.dogecoin_spv_client_new(net, _isDebug, true, true, UseFullSync, _peerCount, null);
            _syncComplete = false;
            IsRunning = false;


            syncTransactionCallback = new dogecoin_spv_client.sync_transaction_delegate(SyncTransaction);
            syncCompletedCallback = new dogecoin_spv_client.sync_completed_delegate(SyncCompleted);
            headerMessageProcessedCallback = new dogecoin_spv_client.header_message_processed_delegate(HeaderMessageProcessed);

            Marshal.WriteIntPtr(_spvNodeRef,
                Marshal.OffsetOf(typeof(dogecoin_spv_client),
                nameof(dogecoin_spv_client.sync_transaction)).ToInt32(), Marshal.GetFunctionPointerForDelegate(syncTransactionCallback));

            Marshal.WriteIntPtr(_spvNodeRef,
                Marshal.OffsetOf(typeof(dogecoin_spv_client),
                nameof(dogecoin_spv_client.sync_completed)).ToInt32(), Marshal.GetFunctionPointerForDelegate(syncCompletedCallback));


            Marshal.WriteIntPtr(_spvNodeRef,
                Marshal.OffsetOf(typeof(dogecoin_spv_client),
                nameof(dogecoin_spv_client.header_message_processed)).ToInt32(), Marshal.GetFunctionPointerForDelegate(headerMessageProcessedCallback));
        }

        private unsafe bool HeaderMessageProcessed(dogecoin_spv_client client, IntPtr node, dogecoin_block_header newtip)
        {
            if (OnProcessedHeaders != null)
            {
                OnProcessedHeaders(new SPVNodeBlockInfo()
                {
                    BlockHeight = (uint)newtip.version
                    ,
                    Hash = ByteArrayToHexString(newtip.prev_block.Reverse().ToArray())

                    // not sure whats going on but it's not correct
                    //,Timestamp = DateTimeOffset.FromUnixTimeSeconds(newtip.timestamp)
                });
            }
            return true;
        }

        private unsafe void SyncCompleted(IntPtr spv)
        {
            _syncComplete = true;
            if (OnSyncComplete != null)
            {
                OnSyncComplete();
            }
        }


        private unsafe void SyncTransaction(IntPtr ctx, IntPtr tx, uint pos, IntPtr blockindex)
        {
            var transaction = Marshal.PtrToStructure<dogecoin_tx>(tx);

            byte[] txHashBytes = new byte[32];
            LibDogecoinInterop.dogecoin_tx_hash(tx, txHashBytes);
            var txId = ByteArrayToHexString(txHashBytes.Reverse().ToArray());

            var blockIdx = Marshal.PtrToStructure<dogecoin_blockindex>(blockindex);
            var blockTimestamp = DateTimeOffset.FromUnixTimeSeconds(blockIdx.header.timestamp);


            if (CurrentBlockInfo == null || CurrentBlockInfo.BlockHeight < blockIdx.height)
            {
                var previousBlock = CurrentBlockInfo;

                CurrentBlockInfo = new SPVNodeBlockInfo
                {
                    BlockHeight = blockIdx.height,
                    Hash = LittleEndianByteArrayToHexString(blockIdx.hash),
                    Timestamp = blockTimestamp,
                };

                BeforeOnNextBlock(previousBlock, CurrentBlockInfo);
            }

            var nodeTransaction = new SPVNodeTransaction();
            var inList = new List<UTXO>();
            var outList = new List<UTXO>();

            nodeTransaction.TxId = txId;
            nodeTransaction.BlockHeight = blockIdx.height;
            nodeTransaction.Timestamp = blockTimestamp;


            var txcstr = LibDogecoinInterop.cstr_new_sz(1024);

            LibDogecoinInterop.dogecoin_tx_serialize(txcstr, (dogecoin_tx*)tx);

            nodeTransaction.SizeBytes = (int)txcstr->len;

            LibDogecoinInterop.cstr_free(txcstr, 1);

            //handle inputs
            var vinList = *transaction.vin;
            for (var i = 0; i < vinList.len; i++)
            {
                dogecoin_tx_in vin = Marshal.PtrToStructure<dogecoin_tx_in>(*(vinList.data + i));

                inList.Add(new UTXO
                {
                    TxId = ByteArrayToHexString(vin.prevout.hash.Reverse().ToArray()),
                    VOut = (int)vin.prevout.n,
                    BlockHeight = blockIdx.height
                });
            }


            //handle outputs
            var voutList = *transaction.vout;

            for (int i = 0; i < voutList.len; i++)
            {
                dogecoin_tx_out vout = Marshal.PtrToStructure<dogecoin_tx_out>(*(voutList.data + i));

                outList.Add(new UTXO
                {
                    TxId = txId,
                    VOut = i,
                    AmountKoinu = vout.value,
                    ScriptPubKey = vout.script_pubkey,
                    BlockHeight = blockIdx.height
                });
            }

            nodeTransaction.In = inList.ToArray();
            nodeTransaction.Out = outList.ToArray();

            if (OnTransaction != null)
            {
                OnTransaction(nodeTransaction);
            }
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static string LittleEndianByteArrayToHexString(byte[] bytes)
        {
            if (bytes == null)
                return null;

            StringBuilder sb = new StringBuilder();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private static byte[] HexStringToLittleEndianByteArray(string hex)
        {
            if (hex == null)
                return null;

            hex = hex.ToUpper().Trim();

            if (hex.Length % 2 == 1)
                hex = "0" + hex;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            Array.Reverse(bytes);
            return bytes;
        }
    }
}