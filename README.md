# Sistema-Distribuido
Projeto de sistema distribuído da UFRPE

Esse projeto busca desenvolver um jogo de maneira distribuida, a base da implementação será um jogo desenvolvido no Unity utilizando C# em conjunto com um servidor
desenvolvido em javascript, a conexão feita com websockts.

O jogo sera baseado no jogo de tabuleiro Muchiking, a ideia a desenvolver uma versão do jogo funcional com a possibilidade de ser jogado em uma conexão local,
no unity sera usada a blibioteca Quobject.SocketIoClientDotNet.Client para lidar com o envio e recibimento de dados.

No servidor sera usada a blibioteca socket.io e a porta usada sera a porta 4080, o servidor ira identificar quando um novo jogador conecta no servidor
e checa se existe um lobby disponivel, caso não exista um novo lobby sera criado, quando um lobby e criado ele ira carregar as cartar a partir de um json
onde as cartas serão definidas e sera responsavel pela logica de turnos do jogo.

----Regras basicas do jogo------
Quando todos os jogadores estiverem prontos o jogo ira selecionar um jogador para começar, o jogador podera equipar itens e desequipar ate puxar a carta porta
quando puxar a carta porta, depois que a carta seja resolvida o jogador sofre as consequencias ou pega as recompensas caso seja vitorioso, apos isso o proximo jogador
puxa uma porta, o primeiro jogador a chegar no nivel 10 ganha.

![image](https://user-images.githubusercontent.com/65423512/232090562-9d8286e8-5cf7-4639-9dca-0f5ef962b207.png)
