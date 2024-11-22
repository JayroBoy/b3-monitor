# b3-monitor
Aplicação de _console_ feita para monitorar um ativo específico na B3 e avisar, por e-mail, caso esse ativo saia de um _range_ informado no momento de início da execução.

_Console application designed to monitor whether a specific token in the brazilian stock exchange(b3) stays within a given range, and send email notifications if the token leaves this range_
# Instruções
Para testar o programa, basta baixar o conteúdo da pasta ```b3-monitor/executavel``` e preencher o arquivo .config presente nela. Depois disso, em um sistema **win-x86**, navegue até o diretório onde o executável e o .config estão e, via linha de comando, invoque-o:

```b3-monitor.exe {token} {preço_venda} {preço_compra} [{frequencia}]```

O parâmetro ```token``` deve ser uma string que represente um ativo na b3.

Os parâmetros ```preço_compra``` e ```preço_venda``` devem ser maiores do que zero, e ambos aceitam tanto ponto quanto vírgula como separador decimal. Além disso, o preço de compra deve ser **menor** do que o preço de venda.

O parâmetro ```frequencia``` é opcional, aceitando os valores ```M1, M5, M10, M15, M30, H1, H2, H3ou H4```, que indicam unidade(M ou H para minutos ou horas) e quantidade de tempo entre verificações. A verificação padrão ocorre de 5 em 5 minutos.

*OBS: A API escolhida consome uma API da B3 cujos valores são atualizados a cada 15 minutos*

# Requisitos
O programa deve ser uma aplicação de console (não há necessidade de interface gráfica).

Ele deve ser chamado via linha de comando com 3 parâmetros:

1 - O ativo a ser monitorado

2 - O preço de referência para venda

3 - O preço de referência para compra

Exemplo:

```> stock-quote-alert.exe PETR4 22.67 22.59``` 

Ele deve ler de um arquivo de configuração com:

1 - O e-mail de destino dos alertas

2 - As configurações de acesso ao servidor de SMTP que irá enviar o e-mail

A escolha da API de cotação é livre.

O programa deve ficar continuamente monitorando a cotação do ativo enquanto estiver rodando.

Exemplo: 
![image](https://github.com/user-attachments/assets/7085cf7b-84fc-43f4-94e0-fc2e55e87226)
Toda vez que o preço for maior que _linha-azul_, um e-mail deve ser disparado aconselhando a venda.

Toda vez que o preço for menor que _linha-vermelha_, um e-mail deve ser disparado aconselhando a compra.


# Detalhes
API de cotação escolhida: GET https://felipemarinho.vercel.app/api/b3/prices/{token}

Payload de resposta:

```
{
    "symbol": "PETR3",
    "name": "PETROBRAS   ON      N2",
    "market": "Vista",
    "openingPrice": 41.43,
    "minPrice": 40.88,
    "maxPrice": 41.7,
    "averagePrice": 41.226,
    "currentPrice": 41.02,
    "priceVariation": -1.2755716,
    "indexComponentIndicator": true
}
```
# Etapas
[x] Configuração básica do projeto

[x] Comunicação com a API de cotação

[x] Envio de e-mail

[x] Monitoramento contínuo

[x] Possibilidade de escolha do token e limitantes superiores e inferiores do intervalo

*[x] Possibilidade de escolha da frequência de verificação*

# Premissas

- O e-mail deve ser enviado sempre que a verificação indicar que o preço atual está fora do intervalo, ainda que na verificação anterior esse já fosse o caso.
- Os valores informados estão inclusos no intervalo. Um ativo cujo preço seja **igual** a algum dos informados pelo usuário não deverá disparar um e-mail
- O programa fará uma nova conferência de preços a cada 1 minuto (M1) *Uma das metas adicionais desse projeto é implementar a possibilidade de escolha dessa frequência*
- Caso o usuário informe um ativo que não existe, o programa deve encerrar a execução.
- Caso o usuário informe um intervalo inverso(Preço de compra maior do que preço de venda), o programa deve encerrar a execução.
