using System;

/*Nicht Benutzbar wegen JsonContainer*/

[Serializable]
public class ReconnectJson
{
    public ContainerJson matchStart;
    public ContainerJson snapshot;
    public ContainerJson next;

    //public ReconnectJson(ContainerJson matchStart, ContainerJson snapshot, ContainerJson next)
    //{
    //    this.matchStart = matchStart;
    //    this.snapshot = snapshot;
    //    this.next = next;
    //}


    //public override string ToString()
    //{
    //    return "ReconnectJson{" +
    //            "matchStart=" + matchStart +
    //            ", snapshot=" + snapshot +
    //            ", next=" + next +
    //            '}';
    //}
}
