var shortID = require('shortid');

module.exports = class Jogador {
    constructor() {
        this.username = '';
        this.id = shortID.generate();
    }
}