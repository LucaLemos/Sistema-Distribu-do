const io = require('socket.io')(4080);
var Jogador = require('./Classes/Jogador');
console.log('Servidor ligadão!!!');

var jogadores = [];
var sockets = [];

io.on('connection', function(socket) {
    console.log('Conectaram :)');
    
    var jogador = new Jogador();
    var thisID = jogador.id;

    jogadores[thisID] = jogador;
    sockets[thisID] = socket;

    socket.emit('register', {id: thisID});
    socket.emit('spaw', jogador);
    socket.broadcast.emit('spaw', jogador);
    
    for(playerID in jogadores) {
      if(playerID != thisID) {
        socket.emit('spaw', jogadores[playerID]);
      }
    }

    socket.on('mover', function(jsonData) {
      const data = JSON.parse(jsonData);
      
      //console.log(data + 'jogador: ' + jogador.id);
      //console.log(data.x);
      //console.log(data.y);
      
      jogador.position.x = data.x;
      jogador.position.y = data.y;
      
      socket.broadcast.emit('mover', jogador);
    });

    socket.on('Explosion', function(jsonData) {
      const data = JSON.parse(jsonData);
      
      console.log('funciona');

      console.log(data);
      //console.log(data.y);
      
      //jogador.position.x = data.x;
      //jogador.position.y = data.y;
      
      socket.broadcast.emit('Explosion', data);
    });
    
    socket.on('disconnect', function() {
      console.log('Desconectaram :(');
      delete jogadores[thisID];
      delete sockets[thisID];
      socket.broadcast.emit('disconnected', jogador)
    });
});