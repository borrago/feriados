# Define a imagem base
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia os arquivos de projeto para o diretório de trabalho
COPY Feriados/Feriados.csproj Feriados/

# Restaura as dependências dos projetos
RUN dotnet restore Feriados/Feriados.csproj

# Copia todo o código-fonte para o diretório de trabalho
COPY . ./

# Publica o projeto principal
RUN dotnet publish Feriados/Feriados.csproj -c Release -o out

# Define a imagem base para a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia os arquivos publicados para o diretório de trabalho
COPY --from=build /app/out ./

# Define o comando de inicialização da aplicação
#ENTRYPOINT ["dotnet", "Feriados.dll"]

# Opção utilizada pelo Heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Feriados.dll
