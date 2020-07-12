# R3M.KafkaMessages
Repositório de estudos usando C# e Kafka.
É necessário .NetCore 3.1 ou superior, docker e docker-compose instalados.

## Rodando os testes
- Acesse a raiz do projeto usando _cmd_ ou o _terminal_
- Execute os comandos abaixo, um por vez
```bash
cd tests/UnitTests/
dotnet test
```
- O resultado vai aparecer na tela

## Rodando a aplicação


### No Linux
É necessário que o sistema tenha o `x-terminal-emulator` instalado. Se esse não for o caso, você pode instalá-lo ou seguir o passo-a-passo para rodar nos [demais sistemas](#demais-sistemas).

- Acesse a raiz do projeto usando o _terminal_
- Execute os comandos abaixo, um por vez, para dar permissão de execução aos _scripts_
```bash
chmod 0755 start.sh
chmod 0755 stop.sh
```
- Execute o comando abaixo. O parâmetro _1_ que passamos indica que o script vai criar 2 programas (1 par), e cada um vai ler e escrever no tópico do outro.
```bash
./start.sh 1
```
- Espere um tempo e veja os programas interagindo entre si.
- Quando quiser parar, execute o comando
```bash
./stop.sh
```

### Demais sistemas

Na raiz do seu projeto, abra um _cmd_, _terminal_ ou similar.

Para iniciar o contêiner rodando Kafka, execute os dois comandos abaixo.

```bash
docker-compose rm -svf
docker-compose up --build -d
```

O primeiro comando evita que outras execuções do Kafka atrapalhem a nossa, evitando o problema de [node already exists](https://github.com/wurstmeister/kafka-docker/issues/389); o segundo comando levanta o Kafka local.

Agora rode os comandos abaixo

```bash
cd WritRead
dotnet build
```

O nosso programa precisa de três parâmetros. O nome do serviço atual, o nome do tópico que ele deve escrever e o nome do tópico que ele vai ler.

```bash
dotnet run ESPIRIQUIDIBERTO PERGAMINHO GRIMOIRE
```

Esse programa está escrevendo no tópico PERGAMINHO e consumindo do tópico GRIMOIRE. Se passarmos o mesmo nome de tópico, significa que ele vai ler e produzir e consumir do mesmo tópico.

Para finalizar o teste, abra outro _cmd_ ou _terminal_ na raiz do projeto (não feche o anterior), e execute os comandos abaixo, um por vez.

```bash
cd WritRead
dotnet run CHAFUNDIFORNIO GRIMOIRE PERGAMINHO
```

As duas janelas deverão mostrar as mensagens umas das outras.

Para finalizar o programa, aperte a combinação CONTROL+C (ou equivalente) e o programa será fechado com segurança.