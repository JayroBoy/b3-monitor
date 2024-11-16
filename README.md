# b3-monitor
Aplicação de _console_ feita para monitorar um ativo específico na B3 e avisar, por e-mail, caso esse ativo saia de um _range_ informado no momento de início da execução.

_Console application designed to monitor whether a specific token in the brazilian stock exchange(b3) stays within a given range, and send email notifications if the token leaves this range_

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
API de cotação escolhida: GET https://cotacao.b3.com.br/mds/api/v1/instrumentQuotation/{token}

Payload de resposta:

```{
    "BizSts": {
        "cd": "OK"
    },
    "Msg": {
        "dtTm": "2024-11-15 15:12:14"
    },
    "Trad": [
        {
            "scty": {
                "SctyQtn": {
                    "opngPric": 39.82,
                    "minPric": 39.82,
                    "maxPric": 40.67,
                    "avrgPric": 40.426,
                    "curPrc": 40.54,
                    "prcFlcn": 1.5276734
                },
                "mkt": {
                    "nm": "Vista"
                },
                "symb": "PETR3",
                "desc": "PETROBRAS   ON      N2",
                "indxCmpnInd": true
            },
            "ttlQty": 12779
        }
    ]
}
```
# Etapas
[ ] Configuração básica do projeto

[ ] Comunicação com a API de cotação

[ ] Envio de e-mail

[ ] Monitoramento contínuo

[ ] Possibilidade de escolha do token e limitantes superiores e inferiores do intervalo

*[ ] Possibilidade de escolha da frequência de verificação - Se der tempo*

# Premissas

- O e-mail só deve ser enviado uma vez para cada saída do intervalo. Ou seja, um ativo que caia abaixo do intervalo e fique abaixo durante 12h só deverá disparar um e-mail
- Os valores informados estão inclusos no intervalo. Um ativo cujo preço seja **igual** a algum dos informados pelo usuário não deverá disparar um e-mail
- O programa fará uma nova conferência de preços a cada 15 minutos (M15) em função do delay da  *Uma das metas adicionais desse projeto é implementar a possibilidade de escolha dessa frequência*
- Caso o usuário informe um ativo que não existe, o programa deve encerrar a execução.
