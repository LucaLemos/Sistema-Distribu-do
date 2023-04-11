var shortID = require('shortid');
var Vector2 = require('./Vector2');
var Corpo = require('./Corpo');

module.exports = class Jogador {
    constructor() {
        this.id = shortID.generate();
        this.lobby = 0;
        this.username = 'Default';
        this.level = new Number(1);
        this.power = new Number(100);
        this.corpo = new Corpo();
        this.position = new Vector2();
        this.isDead = false;
    }

    displayerPlayerInformation() {
        let jogador = this;
        return '(' + jogador.username + ':' + jogador.id + ')';
    }

    dealEffect(effect = Number) {
        this.power = this.power + effect;
    }
}