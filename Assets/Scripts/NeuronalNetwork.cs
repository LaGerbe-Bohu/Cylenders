using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Inputs
{
    public List<float> intputs;
    public List<float> output;
}

[DataContract]
public class saveNN
{
    [DataMember]
    public List<List<float>> wi;
    
    [DataMember]
    public List<List<float>> wo;
    
}

public class NN
{
    private int ni;
    private int no;
    private int nh;
    
    private List<float> ai;
    private List<float> ao;
    private List<float> ah;

    public List<List<float>> wi;
    public List<List<float>> wo; 
    
    private List<List<float>> ci;
    private List<List<float>> co;

    public NN(NN nn)
    {
        this.ni = nn.ni;
        this.nh = nn.nh;
        this.no = nn.no;

        ai = nn.ai;
        ah = nn.ah;
        no = nn.no;

        wi = nn.wi;
        wo = nn.wo;

        ci = nn.ci;
        co = nn.co;
    }
    
    public NN(int _ni, int _nh, int _no)
    {
        this.ni = _ni+ 1;
        this.nh = _nh;
        this.no = _no;
        
        ai = Initizateur(ni, 1.0f);
        ah = Initizateur(nh, 1.0f);
        ao = Initizateur(no, 1.0f);

        wi = makeMatrice(ni, nh);
        wo = makeMatrice(nh, no);

        for (int i = 0; i < ni; i++)
        {
            for (int j = 0; j < nh; j++)
            {
                wi[i][j] = Random.Range(-2.0f, 2.0f);
            }
        }
        
        for (int i = 0; i < nh; i++)
        {
            for (int j = 0; j < no; j++)
            {
                wo[i][j] = Random.Range(-2.0f, 2.0f);
            }
        }

        ci = makeMatrice(ni, nh,0.0f);
        co = makeMatrice(nh, no,0.0f);
        
    }

    public List<float> Update(List<float> inputs)
    {
        if (inputs.Count != ni - 1)
        {
            Debug.LogError("Wrong number of argument");
        }

        for (int i = 0; i < ni-1; i++)
        {
            ai[i] = inputs[i];
        }

        for (int j = 0; j < nh; j++)
        {
            float sum = 0.0f;
            for (int i = 0; i < ni; i++)
            {
                sum += ai[i] * wi[i][j];
            }

            ah[j] = Sigmoid(sum);
        }
        
        
        for (int k = 0; k < no; k++)
        {
            float sum = 0.0f;
            for (int i = 0; i < nh; i++)
            {
                sum += ah[i] * wo[i][k];
            }

            ao[k] = Sigmoid(sum);
        }

        return ao;
    }

    public float BackPropagation(List<float> targets, float N,float M)
    {
        if (targets.Count != no)
        {
            Debug.LogError("Wrong number of argument");
        }

        List<float> output_delta = Initizateur(no,0.0f);
        for (int k = 0; k < no; k++)
        {
            float error = targets[k] - ao[k];
            output_delta[k] = dsigmoid(ao[k]) * (error);
        }


        List<float> hidden_deltas = Initizateur(nh,0.0f);

        for (int j = 0; j < nh; j++)
        {
            float error = 0.0f;
            for (int k = 0; k < no; k++)
            {
                error += output_delta[k] * wo[j][k];
                hidden_deltas[j] = dsigmoid(ah[j]) * error;
            }
            
        }

        for (int j = 0; j < nh; j++)
        {
            for (int k = 0; k < no; k++)
            {
                float change = output_delta[k] * ah[j];
                wo[j][k] = wo[j][k] + N * change + M * co[j][k];
                co[j][k] = change;
            }
        }

        for (int i = 0; i < ni; i++)
        {
            for (int j = 0; j < nh; j++)
            {
                float change = hidden_deltas[j] * ai[i];
                float w = wi[i][j];
                float h = (N * change);
                float m = (M * ci[i][j]);
                wi[i][j] =w +h + m;
                ci[i][j] = change;
            }
        }

        float err = 0.0f;
        for (int k = 0; k < targets.Count; k++)
        {
            err += .5f*((targets[k]-ao[k])*(targets[k]-ao[k]));
        }

        return err;
    }
    
    public void train(List<Inputs> lstInputs,int iteration = 1000,float N=0.5f,float M=0.1f)
    {
   
        for (int i = 0; i < iteration; i++)
        {
            float error = 0.0f;
            for (int j = 0; j < lstInputs.Count; j++)
            {
                List<float> inputs = lstInputs[j].intputs;
                List<float> targets = lstInputs[j].output;
                
                Update(inputs);
                error += BackPropagation(targets,N,M);

             
            }
        }
    }
    
    public List<List<float>> makeMatrice(int i, int j, float fill = 1.0f)
    {
        List<List<float>> output = new List<List<float>> (i);
        for (int k = 0; k < i; k++)
        {
            output.Add( Initizateur(j, fill));
        }

        return output;
    }

    public List<float> Initizateur(int i, float fill)
    {
        List<float> output = new List<float>(i);
        
        for (int j = 0; j < i; j++)
        {
            output.Add(fill);
        }

        return output;
    }
    
    public static float Sigmoid(double x)
    {
        return (float)Math.Tanh(x);
    }

    public static float dsigmoid(float y)
    {
        return (1.0f - y * y);
    }
        

    public void test(List<Inputs> Inputs)
    {
        for (int i = 0; i < Inputs.Count; i++)
        {
            Debug.Log(Inputs[i].intputs[0]+ " , "+Inputs[i].intputs[1] + " -> "+ Update(Inputs[i].intputs)[0] + " ; "+ Update(Inputs[i].intputs)[1] );
        }
    }

}




public class NeuronalNetwork : MonoBehaviour
{
    public List<Inputs> inputs;
    public GameObject Target;
    private NN nn;
    // Start is called before the first frame update
    void Start()
    {
        
        nn = new NN(inputs[0].intputs.Count, 2, inputs[0].output.Count);
        nn.train(inputs);
        nn.test(inputs);

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = this.transform.InverseTransformPoint(Target.transform.position);
        List<float> d = new List<float>() {pos.x,pos.z};
        List<float> d2 = nn.Update(d);
        Vector3 dir = new Vector3(d2[0], 0.0f, d2[1]);
        this.transform.position =
            Vector3.MoveTowards(this.transform.position, this.transform.position + dir.normalized, Time.deltaTime * 1f);
    }
}
