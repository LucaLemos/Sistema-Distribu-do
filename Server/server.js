const io = require('socket.io')(4080);
const fs = require('fs');
var Carta = require('./Classes/Carta');
var Jogador = require('./Classes/Jogador');
console.log('Servidor ligadão!!!');

var jogadores = [];
var sockets = [];
var cartas = [];

fs.readFile('Json/Cards.json', 'utf8', (err, data) => {
  if (err) {
    console.error(err);
    return;
  }

  const myList = JSON.parse(data);

  myList.forEach(item => {
    const cardTest = new Carta();
    cardTest.id = item.id;
    cardTest.image = item.image;
    cardTest.type = item.type;
    cardTest.power = item.power;
    cardTest.effect = item.effect;
    cardTest.equip = item.equip;
    cardTest.monster = item.monster;
    cardTest.treasure = item.treasure;
    cardTest.level = item.level;
    cartas.push(cardTest);
  });
});


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

    socket.emit('getCard', cartas[0]);
    socket.emit('getCard', cartas[1]);
    socket.emit('getCard', cartas[2]);
    socket.emit('getPorta', cartas[2]);

    socket.on('mover', function(jsonData) {
      const data = JSON.parse(jsonData);
      
      jogador.position.x = data.x;
      jogador.position.y = data.y;

      console.log(data);
      
      socket.broadcast.emit('mover', jogador);
    });

    socket.on('explosion', function(jsonData) {
      const data = JSON.parse(jsonData);
      
      socket.broadcast.emit('explosion', data);
    });

    socket.on('effect', function(jsonData) {
      const data = JSON.parse(jsonData);
      
      jogadores[data.id].dealEffect(data.power);
      //console.log(data);
      //console.log(jogadores[data.id]);

      socket.emit('effect', jogadores[data.id]);
      socket.broadcast.emit('effect', jogadores[data.id]);
    });
    
    socket.on('disconnect', function() {
      console.log('Desconectaram :(');
      delete jogadores[thisID];
      delete sockets[thisID];
      socket.broadcast.emit('disconnected', jogador);
    });
});