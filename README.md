![image](https://github.com/UsaRandom/SimpleDogeWallet/assets/2897796/13cf2ead-92dc-470d-b64c-c0ac2d160831)

# SimpleDogeWallet

SimpleDogeWallet is a hot🔥, spv-node powered, self-custody, dogecoin wallet made using *Monogame* and *[libdogecoin](https://github.com/dogecoinfoundation/libdogecoin)*.



# Important Notes

* Very early. Keep that in mind.
* This hot wallet doesn't rely on any additional infrastructure beyond the dogecoin network but doesn't have the growing space requirement of a full node. 
* Can load 12 or 24 word backup phrases.
* Keeps your keys secure *at rest* with TPM2.
* Rescanning with the SPV Node allows you to start scanning for UTXOs at a point on the chain. If you make a new wallet, you can use this to 'skip ahead' to the current chain tip by getting the tip's block hash and height.

# Building Yourself

* "**Build libdogecoin**" dogecoin.dll and sendtx.exe come from libdogecoin. build them with win32, tpm2, net, and tools flags. Currently built with 0.1.4 + PR #205 + #207. 
* "**Build with Visual Studio**"


# Security

Your backup phrases are secured *at-rest* using TPM2, so it should be about as risky as transacting with a phone's hot wallet.

--------
![image](https://github.com/UsaRandom/SimpleDogeWallet/assets/2897796/8ddff94d-bce5-49e8-bc9e-e38ebb2053e2)


![image](https://github.com/UsaRandom/SimpleDogeWallet/assets/2897796/40c8d513-1221-4289-a67f-c4ed9c5cdb6d)
