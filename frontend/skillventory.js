class Skillventory {
  constructor() {
    this.skills = new Map();
  }

  addXp(skillId, amount) {
    if (this.skills.has(skillId)) {
      this.skills.set(skillId, this.skills.get(skillId) + amount);
    } else {
      this.skills.set(skillId, amount);
    }
  }
  getXp(skillId) {
    if (this.skills.has(skillId)) {
      return this.skills.get(skillId);
    }
    return 0;
  }
  log() {
    console.log("__Skillventory__");
    this.skills.keys().forEach((key) => {
      console.log("SkillId: " + key + " Xp: " + this.skills.get(key));
    });
  }
}

var skillventory = new Skillventory();
