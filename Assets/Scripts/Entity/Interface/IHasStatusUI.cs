using System.Collections.Generic;

public interface IHasStatusUI
{
    void BindStatusUI(StatusUI statusUI);
    void UpdateStatusUI(List<BuffBase<Entity>> buffs);
}