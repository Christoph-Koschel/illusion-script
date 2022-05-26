<br/>
<p align="center">
  <a href="https://github.com/christoph-koschel/illusion-script">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Illusion Script</h3>

  <p align="center">
    A simple IL compiled language
    <br/>
    <br/>
    <a href="https://github.com/christoph-koschel/illusion-script"><strong>Explore the docs »</strong></a>
    <br/>
    <br/>
    <a href="https://github.com/christoph-koschel/illusion-script/issues">Report Bug</a>
    .
    <a href="https://github.com/christoph-koschel/illusion-script/issues">Request Feature</a>
  </p>
</p>

![Downloads](https://img.shields.io/github/downloads/christoph-koschel/illusion-script/total) ![Contributors](https://img.shields.io/github/contributors/christoph-koschel/illusion-script?color=dark-green) ![Issues](https://img.shields.io/github/issues/christoph-koschel/illusion-script) ![License](https://img.shields.io/github/license/christoph-koschel/illusion-script)

## Table Of Contents

* [About the Project](#about-the-project)
* [Built With](#built-with)
* [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [Usage](#usage)
    * [Compilers](#compilers)
* [Contributing](#contributing)
* [License](#license)
* [Authors](#authors)

## About The Project

Illusion Script is a multi-compiled scripting language. Illusion Script represents an engine that can compile/transpile scripts into any language for which a compiler is available.

During development it is important for us to keep the language simple and flexible. Nevertheless, it should be clear and safe. Type checking.
## Built With

C#

## Getting Started

This is an example to compile a ils program

### Prerequisites

Download the latest ISC or ISI Release from Illusion Script

ISC (Illusion Script Compiler) is the actual compiler interface.<br>
Through it scripts can be compiled via the console<br>
<br>
ISI (Illusion Script Interface) is a simple console editor for executing code. The code is not directly compiled but directly interpreted by the ILS runtime.<br>
<br>
**Note**: ISI should only be used for small projects or to try things out.

### Using

1. Create a file with the .ils extension
2. Open it in a editor your choose and write:

```
print("Hello World");
```

3. Compile the program

```
ils -c main.ils
```

Additionally, instead of a file, you can also specify a folder as a path. The runtime will filter all *.ils files in the folder and its subfolders out.

```
ils -c src
```

In this example, no target was set, so the runtime executes the code directly.
So no compiled files will be generated.<br>
<br>

To generate compiled files a compiler must be downloaded and placed in the /etc folder.

> CLI support will be available soon.

Additionally the target attribute must be added when compiling.


```
ils --target php8 -c src
```

### Compilers
* [php8](https://github.com/Christoph-Koschel/illusion-script-php-compiler)
* [bcc](https://github.com/Christoph-Koschel/illusion-script-bcc-compiler)

> More are coming soon.

## Contributing

Pull Request are at any time welcome.

Please sort your pull request into categories.

For example:\
bugfixes/TokenLexing\
features/BinaryOperator\
..\
..

### Creating A Pull Request

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See [LICENSE](https://github.com/christoph-koschel/illusion-script/blob/main/LICENSE.md) for more information.

## Authors

* **Christoph Koschel** - *A passionate fullstack developer from Germany* - [Christoph Koschel ](https://github.com/Christoph-Koschel) - *Creator of IllusionScript*
