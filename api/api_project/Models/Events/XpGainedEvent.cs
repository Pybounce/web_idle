
public struct XpGainedEvent: IEvent
{
    public int SkillId { get; set; }
    public int Amount { get; set; }
    public XpGainedEvent(int skillId, int amount) {
        SkillId = skillId;
        Amount = amount;
    }
}