const WebSocket = require('ws');


const wss = new WebSocket.Server({port: 4080}, () => {
    console.log("Server ta funcionando!");
});

wss.on('connection', function connection(ws) {
     ws.on('message', (data) => {
         console.log("recebi: " + data);
         ws.send(data.toString())
     })
 });
 
wss.on('listening', () => {
    console.log("Server na porta 4080!")
});
