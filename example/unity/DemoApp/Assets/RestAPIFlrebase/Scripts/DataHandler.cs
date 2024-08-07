using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{

    public string projectID = "robinsonheli-45b47";
    public string databaseURL = "https://robinsonheli-45b47-default-rtdb.firebaseio.com/";

    public delegate void PostUserCallback();

    public delegate void GetUserCallback(User user);

    private fsSerializer serializer = new fsSerializer();
    public delegate void GetUsersCallback(Dictionary<string, User> users);


    // UI elements

    // Post UI
    public TMP_InputField firstname;
    public TMP_InputField lastname;


    // GET UI
    public TMP_InputField dataID;
    public TextMeshProUGUI outputText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Adds a user to the Firebase Database
    /// </summary>
    /// <param name="user"> User object that will be uploaded </param>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    public void PostUser(User user, string userId, PostUserCallback callback)
    {
        RestClient.Put<User>($"{databaseURL}users/{userId}.json", user).Then(response => {
            
            callback();
            Debug.Log("The user was successfully uploaded to the database");
        });
    }


    /// <summary>
    /// Retrieves a user from the Firebase Database, given their id
    /// </summary>
    /// <param name="userId"> Id of the user that we are looking for </param>
    /// <param name="callback"> What to do after the user is downloaded successfully </param>
    public void GetUser(string userId, GetUserCallback callback)
    {
        RestClient.Get<User>($"{databaseURL}users/{userId}.json").Then(user =>
        {
            callback(user);
            Debug.Log("The user was successfully recieved from the database");
        });
    }


    /// <summary>
    /// Gets all users from the Firebase Database
    /// </summary>
    /// <param name="callback"> What to do after all users are downloaded successfully </param>
    public void GetUsers(GetUsersCallback callback)
    {
        RestClient.Get($"{databaseURL}users.json").Then(response =>
        {
            var responseJson = response.Text;
            // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
            // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, User>), ref deserialized);
            var users = deserialized as Dictionary<string, User>;
            callback(users);
        });
    }


    // Post Button Click
    public void PostButton()
    {
        if (outputText != null)
        {
            outputText.text = "";
        }
        // post button
        if (firstname == null || lastname == null)
        {
            Debug.Log("Assign the Input fields to script");
            return;
        }

        if (String.IsNullOrEmpty(firstname.text) || String.IsNullOrEmpty(lastname.text))
        {
            if (outputText != null)
            {
                outputText.text = "Enter both your First and Last name";
            }

        }
        else
        {
            User user = new User(firstname.text,lastname.text,DateTime.Today.Day);
            string userid = "" + firstname.text + lastname.text + DateTime.Today.Day;
            PostUser(user,userid,() =>
            {
                if (outputText != null)
                {
                    outputText.text = "The user was successfully uploaded to the database";
                }
            });
        }
    }

    public void GetButton()
    {
        if (outputText != null)
        {
            outputText.text = "";
        }

        // get button
        if (dataID != null && !String.IsNullOrEmpty(dataID.text))
        {
            GetUser(dataID.text, user =>
            {
                if (outputText != null)
                {
                    outputText.text = $"{user.name} {user.surname} {user.age}";
                }
                
                Debug.Log($"{user.name} {user.surname} {user.age}");

            });

        }
        else
        {
            // The dataID Input field is empty
            GetUsers(users =>
            {
                foreach (var user in users)
                {
                    if (outputText != null)
                    {
                        outputText.text += $"{user.Value.name} {user.Value.surname} {user.Value.age}\n\n";
                    }
                    Debug.Log($"{user.Value.name} {user.Value.surname} {user.Value.age}");
                }
            });

        }

    }
}
