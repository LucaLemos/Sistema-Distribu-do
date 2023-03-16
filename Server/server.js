const io = require('socket.io')(4080);
var Jogador = require('./Classes/Jogador');
console.log('Servidor ligadão :)');

var jogadores = [];
var sockets = [];

io.on('connection', function(socket) {
    console.log('Conectaram!!!');
    
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
    
    socket.on('disconnect', function() {
      console.log('Foram embora ):');
      delete jogadores[thisID];
      delete sockets[thisID];
      socket.broadcast.emit('disconnected', jogador)
    });
});