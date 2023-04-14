let io = require('socket.io')(4080);
let Servidor = require('./Classes/Servidor');
const fs = require('fs');
var Carta = require('./Classes/Carta');

console.log('Servidor ligadão!!!');

var cartasPorta = [];
var cartasTesouro = [];

fs.readFile('Json/CardsPorta.json', 'utf8', (err, data) => {
  if (err) {
    console.error(err);
    return;
  }

  const myList = JSON.parse(data);

  myList.forEach(item => {
    const cardTest = new Carta();
    cardTest.id = item.id;
    cardTest.image = item.image;
    cardTest.deck = item.deck;
    cardTest.type = item.type;
    cardTest.descricao = item.descricao;
    cardTest.treasure = item.treasure;
    cardTest.level = item.level;
    cardTest.power = item.power;
    cardTest.corpo = item.corpo;
    cardTest.efeito = item.efeito;
    cartasPorta.push(cardTest);
  });
});
fs.readFile('Json/CardsTesouro.json', 'utf8', (err, data) => {
  if (err) {
    console.error(err);
    return;
  }

  const myList = JSON.parse(data);

  myList.forEach(item => {
    const cardTest = new Carta();
    cardTest.id = item.id;
    cardTest.image = item.image;
    cardTest.deck = item.deck;
    cardTest.type = item.type;
    cardTest.descricao = item.descricao;
    cardTest.treasure = item.treasure;
    cardTest.level = item.level;
    cardTest.power = item.power;
    cardTest.corpo = item.corpo;
    cardTest.efeito = item.efeito;
    cartasTesouro.push(cardTest);
  });
});

let server = new Servidor(cartasPorta, cartasTesouro);

setInterval(() => {
  server.onUpdate();
}, 100, 0);

io.on('connection', function(socket) {
  let connection = server.onConnected(socket);
  connection.createEvents();
  connection.socket.emit('register', {'id': connection.player.id});
});
