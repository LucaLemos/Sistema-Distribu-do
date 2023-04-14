module.exports = class Efeito {
    constructor() {
        this.id = '';
        this.level = new Number(0);
        this.power = new Number(0);
        this.treasure = new Number(0);
        this.destruir = false;
        this.monsterOnly = false;
        this.playerOnly = false;
    }
}