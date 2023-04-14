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

        socket.on('pronto', function(Data) {
            connection.player.pronto = Data;
            
            socket.broadcast.to(connection.lobby.id).emit('pronto', connection.player);
        });

        socket.on('jogadorTurno', function(Data) {
            socket.emit('jogadorTurno', connection.lobby.getPlayer(Data));
            socket.broadcast.to(connection.lobby.id).emit('jogadorTurno', connection.lobby.getPlayer(Data));
        });
        socket.on('comecarPorta', function() {
            socket.broadcast.to(connection.lobby.id).emit('comecarPorta');
        });
        socket.on('terminarPorta', function() {
            socket.emit('terminarPorta');
            socket.broadcast.to(connection.lobby.id).emit('terminarPorta');
        });

        socket.on('getCardTesouro', function() {     
            socket.emit('getCard', connection.lobby.getCardTesouro());
        });
        socket.on('getCardPorta', function() {        
            socket.emit('getCard', connection.lobby.getCardPorta());
        });
        socket.on('getPorta', function() {    
            var card = connection.lobby.getCardPorta()
            socket.emit('getPorta', card);
            socket.broadcast.to(connection.lobby.id).emit('getPorta', card);
        });
        socket.on('getRecompensa', function() {    
            var card = connection.lobby.getCardTesouro();
            socket.emit('getRecompensa', card);
        });

        socket.on('effect', function(jsonData) {
            const data = JSON.parse(jsonData);

            connection.lobby.connections.forEach(element => {
                if(element.player.id == data.id) {
                    element.player.dealEffect(data);
                    
                    socket.emit('effect', element.player);
                    socket.broadcast.to(connection.lobby.id).emit('effect', element.player);
                }
            });
        });
        socket.on('zerarPoder', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            connection.lobby.connections.forEach(element => {
                if(element.player.id == data.id) {
                    element.player.setPower(data.power);
                    
                    socket.emit('effect', element.player);
                    socket.broadcast.to(connection.lobby.id).emit('effect', element.player);
                }
            });
        });
        
        socket.on('effectMonster', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            socket.emit('effectMonster', data);
            socket.to(connection.lobby.id).broadcast.emit('effectMonster', data);
        });

        socket.on('explosion', function(jsonData) {
            const data = JSON.parse(jsonData);
            
            socket.broadcast.to(connection.lobby.id).emit('explosion', data);
        });

    }
}