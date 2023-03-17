var shortID = require('shortid');
var Vector2 = require('./Vector2');

module.exports = class Jogador {
    constructor() {
        this.id = shortID.generate();
        this.username = '';
        this.position = new Vector2();
    }
}