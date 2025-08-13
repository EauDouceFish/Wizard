using System.Collections.Generic;

public struct OnAvailableBlessPropsAssignedEvent
{
    public List<AbstractBlessingProp> availableProps;

    public OnAvailableBlessPropsAssignedEvent(List<AbstractBlessingProp> props)
    {
        availableProps = props;
    }
}

public struct BlessPropSelectedEvent
{
    public AbstractBlessingProp SelectedProp;
    public BlessPropSelectedEvent(AbstractBlessingProp selectedProp)
    {
        SelectedProp = selectedProp;
    }
}