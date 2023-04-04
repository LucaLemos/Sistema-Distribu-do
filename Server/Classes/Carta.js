module.exports = class Carta {
    constructor() {
        this.id = '';
        this.image = '';
        this.type = '';
        this.power = new Number(0);
        this.effect = false;
        this.equip = false;
        this.monster = false;
        this.treasure = new Number(0);
        this.level = new Number(0);
    }
}