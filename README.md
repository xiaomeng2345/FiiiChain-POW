# FiiiChain

An easy understanding and customizable blockchain framework based on layered architecture design pattern.

## Abstract

FiiiChain is developed from scratch base on Bitcoin source code using C# programming language. Our aim is to standardize blockchain protocol and modules with the “plug-and-play” capability for developers to build or modify the blockchain characteristic base on different business requirement. Blockchain should not be lableled as rocket science, and shall make it easily learned by anyone. Hence, we sacrificed ourselves gone through the pain of C++ code, rebuild the entire blockchain with C#. 

We welcome all C# developers come and join us together maintain and enhance the blockchain framework.

We have FiiiCoin built in different consensus. If you are looking for Delegate Proof of Capacity consensus, please visit https://github.com/FiiiLab/FiiiCoin

For more information, please visit https://fiii.io

## License

FiiiCoin is released under the terms of the MIT license. Please refer to https://opensource.org/licenses/MIT.

## Pre-requisite

[.NET Core 2.1](https://www.microsoft.com/net/download/dotnet-core/2.1)

## Recommended IDE Tool

[Visual Studio Community 2017](https://visualstudio.microsoft.com/)

[Visual Studio Code](https://visualstudio.microsoft.com/)

## Specification

Coin Name: FiiiCoin

Ticker Symbol: FIII

Programming Language: C#

Digital Signature Algorithm: Ed25519

Consensus Algorithm: Proof of Work

Supply Limit: 4,999,999,999.92724000

Subunits: 1/100000000

Block Time: 60 seconds

Block Size: 12MB

Block Reward: 250 FIII

Reward Halving: 5,000,000 blocks

Hash Function: SHA3-256

Database: SQLite

# Getting Started

For beginner:

1. Download and install the wallet from Release

2. Run the *.exe executable with administrator rights

For expert:

1. Open the project with Visual Studio

2. Run the node by executing the FiiiChain.Node which is located at the Node\Presentation folder

3. Run the wallet by executing the FiiiCoin.Wallet.Win in the Wallet folder

4. Run the mining application by executing the FiiiCoin.Miner in the Wallet folder

# Mining

1. Run the wallet with administrator rights

2. Wait for block synchronization to be complete

3. Click Receive Address from the top menu of the wallet

4. Copy the wallet address

5. Download and extract FiiiCoin.Miner_portable.zip

6. Execute dotnet FiiiCoin.Miner.dll mining <Worker> <Wallet Address>

# Resources

1. [Yellow Paper](https://fiii.io/images/doc/fiiicoin.yellowpaper.v01.pdf)

2. [White Paper](https://fiii.io/images/doc/whitepaper.pdf)

3. [Official Website](https://fiii.io)
