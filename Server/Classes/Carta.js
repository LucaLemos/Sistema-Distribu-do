var Efeito = require('./Efeito');
var Corpo = require('./Corpo');

module.exports = class Carta {
    constructor() {
        this.id = '';
        this.image = '';
        this.deck = ''
        this.type = '';
        this.descricao = '';
        this.treasure = new Number(0);
        this.level = new Number(0);
        this.power = new Number(0);
        this.corpo = new Corpo();
        this.efeito = new Efeito();
    }
}