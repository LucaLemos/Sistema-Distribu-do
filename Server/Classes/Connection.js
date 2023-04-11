module.exports = class Connection {
    constructor() {
        this.socket;
        this.player;
        this.server;
        this.lobby;
    }

    createEvents() {
        let connection = this;
        let socket = connection.socket;
        let server = connection.server;
        let player = connection.player;
        let lobby = connection.lobby;

        socket.on('disconnect', function() {
            server.onDisconnected(connection);
        });

        socket.on('joinGame', function() {
            server.onAttemptToJoinGame(connection);
        });

        socket.on('move', function(jsonData) {
            const data = JSON.parse(jsonData);

            //console.log(data)
            connection.player.position.x = data.x;
            connection.player.position.y = data.y;
                
            socket.broadcast.to(connection.lobby.id).emit('move', connection.player);
        });

        socket.on('getCardTesouro', function() { 
            console.log("get")       
            socket.emit('getCard', connection.lobby.getCardTesouro());
        });
        socket.on('getCardPorta', function() {        
            socket.emit('getCard', connection.lobby.getCardPorta());
        });
        socket.on('getPorta', function() {    
            //console.log("entrou?");
            //console.log(connection.lobby.getCardPorta());
            socket.emit('getPorta', connection.lobby.getCardPorta());
        });

        socket.on('explosion', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            socket.broadcast.to(connection.lobby.id).emit('explosion', data);
        });
        
        socket.on('effect', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            connection.player.dealEffect(data.power);
            socket.emit('effect', connection.player);
            socket.broadcast.to(connection.lobby.id).emit('effect', connection.player);
        });

        socket.on('effectMonster', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            socket.emit('effectMonster', data);
            socket.to(connection.lobby.id).broadcast.emit('effectMonster', data);
        });

    }
}