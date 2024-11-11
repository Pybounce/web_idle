
public struct XpGained: IEvent
{
    public int SkillId { get; set; }
    public int Amount { get; set; }
    public XpGained(int skillId, int amount) {
        SkillId = skillId;
        Amount = amount;
    }
}