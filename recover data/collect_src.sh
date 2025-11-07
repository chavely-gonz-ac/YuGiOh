#!/bin/bash

./collect_files.sh ./src/YuGiOh.Domain "cs csproj"
./collect_files.sh ./src/YuGiOh.Application "cs csproj"
./collect_files.sh ./src/YuGiOh.Infrastructure "cs csproj"
./collect_files.sh ./src/YuGiOh.WebAPI "cs csproj"
./collect_files.sh ./src/YuGiOh.UI/src "json tsx ts md js"