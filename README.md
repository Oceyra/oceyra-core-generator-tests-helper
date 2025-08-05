# Oceyra Generator Test Helper
The goal of this project is to have a permanent route, for ESP devices to fetch a controlled version of their firmware. This way, you can update a single device, or all at once

[![Build status](https://github.com/oceyra/oceyra-core-generator-tests-helper/actions/workflows/publish.yaml/badge.svg?branch=main&event=push)](https://github.com/oceyra/oceyra-core-generator-tests-helper/actions?workflow=publish.yaml)

## Usage Sample
```c#
var result = SourceGeneratorVerifier.CompileAndTest<ConstructorGenerator>(source);

result
    .ShouldHaveNoErrors()
    .ShouldExecuteWithin(TimeSpan.FromMilliseconds(1000))
    .ShouldHaveGeneratorTimeWithin<ConstructorGenerator>(TimeSpan.FromMilliseconds(100))
    .ShouldGenerateFiles(1);
```

## Sponsors

<p><a href="https://www.buymeacoffee.com/pierduchp" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-blue.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a></p>