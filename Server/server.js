let io = require('socket.io')(4080);
let Servidor = require('./Classes/Servidor');
const fs = require('fs');
var Carta = require('./Classes/Carta');

console.log('Servidor ligadão!!!');

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

let server = new Servidor(cartas);

setInterval(() => {
  server.onUpdate();
}, 100, 0);

io.on('connection', function(socket) {
  let connection = server.onConnected(socket);
  connection.createEvents();
  connection.socket.emit('register', {'id': connection.player.id});
});
