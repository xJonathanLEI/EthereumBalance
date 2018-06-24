# Ethereum Balance Finder

Finding ETH or any ERC-20 token balance of an address at any **block** is simple enought: just call `getStorageAt()` with the block number. However, a more practical use case would be to find the balance of an address at a given **time**. It sounds easy but unless you keep a full copy of block timestamps, you'd easily get stucked.

This project aims to develop a web-based tool for doing just that, by fecthing and caching the required block infomation *on-demand*.

This repository includes both the server program built with `.NET Core` as well as the client program built with `React`. Once deployed, a user can use the tool just with a browser.

## Using the Tool

Make sure you have `.NET Core Runtime` of at least `v2.0.0` installed:

    $ dotnet --version

You don't really need `Node.js` to run the program, as the client would be hosted directly by the `Kestrel` server. However, you'd still need `npm` to restore packages after you clone the repository:

    $ npm --version

Clone the repository:

    $ git clone https://github.com/xJonathanLEI/EthereumBalance.git
    $ cd EthereumBalance

Publish the program:

    $ dotnet publish -c Release
    $ cd bin/Release/netcoreapp2.0/publish

Run the program:

    $ dotnet EthereumBalance.dll