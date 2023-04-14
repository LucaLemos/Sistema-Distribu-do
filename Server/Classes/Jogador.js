var shortID = require('shortid');
var Vector2 = require('./Vector2');
var Corpo = require('./Corpo');
const Efeito = require('./Efeito');

module.exports = class Jogador {
    constructor() {
        this.lobby = 0;
        this.id = shortID.generate();
        this.username = 'Default';
        this.level = new Number(1);
        this.power = new Number(1);
        this.gold = new Number(0);
        this.corpo = new Corpo(1, 1, 2, 1, 1);
        this.position = new Vector2();
        this.pronto;
        this.isDead = false;
    }

    displayerPlayerInformation() {
        let jogador = this;
        return '(' + jogador.username + ':' + jogador.id + ')';
    }

    dealEffect(effect = Efeito) {
        if(this.level > 1) {
            this.level = this.level + effect.level;
        }
        this.power = this.power + effect.power;
    }

    setPower(poder) {
        this.power = poder;
    }
}