let LobbyBase = require('./LobbyBase');
let GameLobbySetting = require('./GameLobbySetting');
let Connection = require('../Connection');

module.exports = class gmaeLobby extends LobbyBase {
    constructor(id, settings = GameLobbySetting, cartasPorta, cartasTesouro) {
        super(id);
        this.settings = settings;
        this.cartasPorta = cartasPorta;
        this.cartasPortaDescarte = [];
        this.cartasTesouro = cartasTesouro;
        this.cartasTesouroDescarte = [];
    }

    onUpdate() {
        let lobby = this;
        
    }

    onEnterLobby(connection = Connection) {
        let lobby = this;
        let socket = connection.socket;

        super.onEnterLobby(connection);
        
        lobby.addPlayer(connection);
        //lidar com o spaw de objetos
        //socket.emit('getCard', this.cards[0]);
        //socket.emit('getCard', this.cards[1]);
        //socket.emit('getCard', this.cards[2]);
        //socket.emit('getCard', this.cards[3]);
        //socket.emit('getPorta', this.cards[4]);
    }

    onLeaveLobby(connection = Connection) {
        let lobby = this;

        super.onLeaveLobby(connection);
        lobby.removePlayer(connection);
        //lidar com o unspaw de objetos
    }

    addPlayer(connection = Connection) {
        let lobby = this;
        let connections = lobby.connections;
        let socket = connection.socket;

        //var returnData = {
        //    id: connection.player.id
        //}

        socket.emit('spaw', connection.player);
        socket.broadcast.to(lobby.id).emit('spaw', connection.player);

        connections.forEach(c => {
            if(c.player.id != connection.player.id) {
                socket.emit('spaw', c.player);
            }
        });
    }

    removePlayer(connection = Connection) {
        let lobby = this;

        console.log("players desconectou " + lobby.id)
        connection.socket.broadcast.to(lobby.id).emit('disconnected', {
            id: connection.player.id
        })
    }

    canEnterLobby(connection = Connection) {
        let lobby = this;
        let maxPlayerCount = lobby.settings.maxPlayers;
        let currentPlayerCount = lobby.connections.length;

        if(currentPlayerCount + 1 > maxPlayerCount) {
            return false;
        }

        return true;
    }

    getCardPorta() {
        var max = this.cartasPorta.length;
        var randomNumber = Math.floor(Math.random() * max);
        var card = this.cartasPorta[randomNumber];
        this.cartasPortaDescarte.push(card);
        this.cartasPorta.splice(this.cartasPorta.indexOf(card), 1);
        
        return card;
    }

    getCardTesouro() {
        var max = this.cartasTesouro.length;
        var randomNumber = Math.floor(Math.random() * max);
        var card = this.cartasTesouro[randomNumber];
        this.cartasTesouroDescarte.push(card);
        this.cartasTesouro.splice(this.cartasTesouro.indexOf(card), 1);

        return card;
    }

}