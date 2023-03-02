using System;
using Newtonsoft.Json;

/*Not in use anymore.
class ClientServer uses methode to generate the container*/

[Serializable]
public class ContainerJson 
{
    public string timestamp;
    public string payloadType;
    public object payload; //Aufpassen kein Jason Object!
}
