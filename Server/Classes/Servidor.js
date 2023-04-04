let Connection = require('./Connection');
let Jogador = require('./Jogador');
const LobbyBase = require('./Lobbies/LobbyBase');
const GameLobby = require('./Lobbies/GameLobby');
const GameLobbySetting = require('./Lobbies/GameLobbySetting');

module.exports = class Servidor {
    constructor(cards) {
        this.connection = [];
        this.lobbys = [];
        this.cards = cards;

        this.lobbys[0] = new LobbyBase(0);
    }
    
    //intervalos de 100 milisegundos de update
    onUpdate() {
        let server = this;

        for(let id in server.lobbys) {
            server.lobbys[id].onUpdate();
        }
    }
    
    //lida com novas conexões
    onConnected(socket) {
        let server = this;
        let connection = new Connection();
        connection.socket = socket;
        connection.player = new Jogador();
        connection.server = server;

        let player = connection.player;
        let lobbys = server.lobbys;
        
        console.log("Novo jogador no server: " + player.id);
        server.connection[player.id] = connection;
        
        socket.join(player.lobby);
        connection.lobby = lobbys[player.lobby];
        connection.lobby.onEnterLobby(connection);

        return connection;
    }

    onDisconnected(connection = Connection) {
        let server = this;
        let id = connection.player.id;

        delete server.connection[id];
        console.log('Player ' + connection.player.displayerPlayerInformation() + ' desconectou');

        //connection.socket.broadcast.to(connection.player.lobby).emit('disconnected', connection.player);

        server.lobbys[connection.player.lobby].onLeaveLobby(connection);
        
    }

    onAttemptToJoinGame(connection = Connection) {
        let server = this;
        let lobbyFound = false;

        let gamelobbies = server.lobbys.filter(item => {
            return item instanceof GameLobby
        });
        console.log("Numero de Lobbies: " + gamelobbies.length);

        gamelobbies.forEach(lobby => {
            if(!lobbyFound) {
                let canJoin = lobby.canEnterLobby(connection);

                if(canJoin) {
                    lobbyFound = true;
                    server.onSwitchLobby(connection, lobby.id);
                }
            }
        });

        if(!lobbyFound) {
            console.log("Criando um novo lobby");
            let gamelobby = new GameLobby(gamelobbies.length + 1, new GameLobbySetting('Padrao', 2), server.cards);
            server.lobbys.push(gamelobby);
            server.onSwitchLobby(connection, gamelobby.id);
        }
    }

    onSwitchLobby(connection = Connection, lobbyID) {
        let server = this;
        let lobbys = server.lobbys;

        connection.socket.join(lobbyID);
        connection.lobby = lobbys[lobbyID];

        lobbys[connection.player.lobby].onLeaveLobby(connection);
        lobbys[lobbyID].onEnterLobby(connection);
    }
}