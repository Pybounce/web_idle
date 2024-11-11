

public struct XpGained {
    public XpGained(int skillId, int amount) {
        MessageType = WriteMessageTypes.XpGained;
        SkillId = skillId;
        Amount = amount;
    }
    public string MessageType { get; set; }
    public int SkillId { get; set; }
    public int Amount { get; set; }
}

