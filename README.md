# VR-Dialogues: Navigating Conversations with Virtual Agents
_By Alen Jo, Evan Olson, Hrishikesh Srirangam, Aastha Sharma, Yadiel Valentin Rios, Abbas Khawaja_

<img width="1282" alt="Screenshot 2024-11-17 at 5 57 11 PM" src="https://github.com/user-attachments/assets/245da27b-0e49-4d75-87e9-143fba1e5db0">

# Introduction
Virtual Reality (VR) has significantly changed how users engage with digital spaces. However, many existing systems focus on one-on-one conversations, failing to address the complexity of multi-agent interactions (Josyula et al., 2024). We introduce VR Dialogues, an immersive VR conversational system that generates realistic, dynamic interactions between a user and two virtual agents. By integrating an LLM, lifelike avatars, and immersive environments, VR Dialogues enhances the virtual reality experience, offering a more collaborative and synergistic form of interaction. This system brings a new level of immersion and interactivity to VR environments, demonstrating the potential for scalable, multi-agent conversations.

# Architecture
![architectureFlowChart](https://github.com/user-attachments/assets/aee01fa0-2678-43ca-b91f-aa1fd6343caa)
<br> _Figure 1. Architecture of VR Dialogues_

The design goal embodies user comfort and control with a seamless user experience and a robust backend. This comfort is what allows the user to enjoy their experience and stimulates more intrigue. To begin, the user enters the game scene facing the agents. The agents will greet the user and start the conversation (e.g., Figure 1). A Speech-To-Text model will process the user’s responses, which feeds the text into an OpenAI Large Language Model (LLM) to generate the agent response. The LLM has multiple custom prompts that allow users to enjoy multiple personalities. In conjunction with these responses, the animations employ advanced lip-syncing techniques to make these agents more lifelike.

***More on the backend***: The flow chart indicates that the backend consists of a Python and C# component. The Python backend first handles the microphone input to detect speech by removing ambient noise, adjusting noise gain, and clipping audio after detecting silence. Then, the recorded audio goes through a transcription layer that converts the audio into proper speech. OpenAI's LLM is the intermediary that connects the speech-to-text and text-to-speech modules by generating the appropriate response and saving it as plaintext. This plaintext then goes through Google's TTS API, which spits out an audio file. Unity's Audio Engine picks up this new audio file through file synchronization of specific characters between the Python and Unity intermediates that then play the audio file in the game scene.

# Components
**Response Generation**: We use GPT-3.5 to generate contextually relevant responses based on user input. In our VR environment, the avatars "Kirtana" and "Ezio" engage in a three-way dialogue with the user, managed by a Python script that syncs with Unity for real-time interaction (e.g., Figure 2). The avatars also have interchangeable personality traits for an adaptive and engaging experience.

***Response generation pipeline***
1. Microphone Input
```py
    audio_data = sd.rec(int(duration * 44100), samplerate=44100, channels=1, dtype='int16')
    sd.wait()  # Wait until the recording is finished
    with wave.open(filename, 'wb') as wf:
        wf.setnchannels(1)  # Mono
        wf.setsampwidth(2)  # 16 bits
        wf.setframerate(44100)
        wf.writeframes(audio_data.tobytes())
```
Take in a sound device and instantiate input properties, such as the sample rate at 44100 Hz, mono-channel, and 16-bit audio blocks. 44100 Hz comes from a fantastic theorem (google the Nyquist-Shannon sampling principle) that captures frequencies up to 22 kHz (above the upper limit for human hearing). Therefore, to add cushioning for human hearing, double it. A mono-channel configuration allows us to make the audio file small for more straightforward encoding and decoding. Finally, 16-bit is lossless or allows for leeway of better speech recognition and toying around with loudness in certain audio blocks. 

We then use a context manager to encode <--> decode the microphone audio and write as bytes into a WAVE file. WAVE files are made purely using raw bytes, which means they have pure sound quality (excellent for transcription).

2. Speech-To-Text Transcriber
```py
    recognizer = sr.Recognizer()
    with sr.Microphone() as source:
        print("Listening...")
        audio = recognizer.listen(source, timeout=2)
```
Instantiate a Speech Recognition object that includes the microphone array to take in audio input continuously until the recognition model detects speech from `py recognizer.recognize_google(audio)`. Since speech recognition is susceptible to errors, there are also three defined Exception classes:
```py
        except sr.UnknownValueError:
            print("Sorry, I could not understand the audio.")
            return ""
        except sr.RequestError as e:
            print(f"Could not request results from Google Speech Recognition service; {e}")
            return ""
        except sr.WaitTimeoutError:
            return ""
```
`UnknownValueError` - failure in transcribing the user audio, which this error is harmless and warrants no response.
`RequestError` - failure in fetching the transcription service through its internal API
`WaitTimeOutError` - reached the silence detection threshold that prematurely clips the microphone input

3. OpenAI LLM Intermediary
```py
            # Append user input to both conversations
            conversation1.append({'role': 'user', 'content': user_input})
            conversation2.append({'role': 'user', 'content': user_input})

            # Randomly select an agent to respond
            agentSelected = random.randint(1, 2)

            # Get the appropriate response from OpenAI
            response = (gpt3(conversation1) if agentSelected == 1 else gpt3(conversation2))
```
Once the speech-to-text extracts the appropriate transcription, feed it into a list of JSON objects fed into the OpenAI API. To prevent monotonicity in the agent, randomize the agent to give the user comfort in speaking to different people rather than a single person. Check the following for how we invoke the OpenAI LLM:
```py
        response = openai.ChatCompletion.create(
            model=model,
            messages=messages,
            temperature=temperature,
            max_tokens=max_tokens,
            frequency_penalty=frequency_penalty,
            presence_penalty=presence_penalty
        )
        response_text = response['choices'][0]['message']['content'].strip()
        return response_text
```
To get an appropriate response from OpenAI's ChatGPT model, we need to invoke a ChatCompletion function that allows for selecting the model (i.e., GPT 3.5), the messages (List of JSON objects), temperature (model's creativity), max_tokens (how many characters allowed for response), frequency penalty (prevents repeating the exact phrases or words), and presence penalty (be more diverse in the conversation).

All of these parameters make agent responses genuine and realistic, and the successful generation returns as plaintext. Of course, there will be errors encountered (like invalid API requests), which are handled like so.
```py
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return "I'm having trouble responding right now."
```

4. Text-to-Speech Transcriber
For Kitana:
```py
        text_to_wav("en-US-Standard-H", response, "girlAudio")  # Female voice for Kitana
        conversation1.append({'role': 'system', 'content': response})
        conversation2.append({'role': 'user', 'content': f'Kitana said "{response}"'})
        with open("sync.txt", "a") as sync_file:
            sync_file.write("a1\n")
```

With Kitana, we invoke a text_to_wav function that takes the standard US English locale, the response, and then the audio name. The response manipulates the conversation lists that pass as JSON objects for the system (OpenAI's LLM) and user (agent). Finally, write a sync file that tells the TTS module it's ready to play the audio!

```py
        text_to_wav("en-US-Standard-J", response, "boyAudio")  # Male voice for Ezio
        conversation1.append({'role': 'user', 'content': f'Ezio said "{response}"'})
        conversation2.append({'role': 'system', 'content': response})
        with open("sync.txt", "a") as sync_file:
            sync_file.write("a2\n")
```

However, notice the Ezio code block. The difference derives from which role goes first. In Kitana, it's the LLM response and then the agent response. However, Ezio is the agent response, followed by the LLM response. This design choice allows for a two-way conversation (alludes to the next section) between the two agents and the user, creating a social ambiance!

5. Two-way conversation between the agents
```py
            agent_count = 0
            while agent_count < 3:
                agentSelected = 2 if agentSelected == 1 else 1  # Switch agent
                response = (gpt3(conversation1) if agentSelected == 1 else gpt3(conversation2))
                tts(response, agentSelected)
                agent_count += 1
```

Remember how we want to avoid monotonicity (or redundancy rather) for one agent speaking the entire time? This allows variance in which the agent speaks first and gives that social creativity! It's not enough to personalize the LLM models to provide human-like responses; we need to "physically" mimic the social interaction seen routinely.



**Avatars and Animations**: Our agents’ avatars were designed with Ready Player Me and animated using Mixamo and custom Blender animations (e.g., Figure 3 \& 4). This approach enhances avatar liveliness, promoting user social presence (Qazi \& Qazi, 2023). Animations and expressions are dynamically adjusted based on speech sentiment, enabling natural interactions. Each agent (Ezio and Kirtana) has 8 avatars each. Upon colliding with the respective clothing item in the wardrobe corresponding to some avatar, their speech prompt will be adjusted to a new personality and the skin child that matches the clothing will be set to active while the previously active skin child will be set to inactive. With this active-inactive infrastructure in each avatar's children, many avatars can be present in the scene with all their components without any conflict.

<table>
  <tr>
    <td style="text-align: center;">
      <img src="https://github.com/user-attachments/assets/ab7f3372-f7d3-4bb3-bf5c-a6210b3e2532" style="height: 200px;">
      <br>
      <em>Figure 2. Agents Talking</em>
    </td>
    <td style="text-align: center;">
      <img src="https://github.com/user-attachments/assets/d917f5b3-efd0-49f3-a5ef-86abb6317a97" style="height: 200px;">
      <br>
      <em>Figure 3. Kirtana</em>
    </td>
    <td style="text-align: center;">
      <img src="https://github.com/user-attachments/assets/c18169bb-203b-48bd-a47b-9ff0f5f2c799" style="height: 200px;">
      <br>
      <em>Figure 4. Ezio Clapping</em>
    </td>
  </tr>
</table>

**Environment**: To suit various preferences, we included a park environment in addition to a more enclosed office environment (Riches et al., 2019). We implemented an elevator system to seamlessly transition between these environments (e.g., Figure 5). This is accomplished with buttons that are activated with the user’s hands. When a button is pressed, the door closes, and the elevator is teleported to the corresponding environment, along with the user. Then, the door reopens, revealing the new environment. This method of transporting the user hides any jarring transitions from the user. Additionally, we added interactable elements to the scenes to enhance the user's sense of presence within the scene. For example, the items on the desk in the office can be moved, shattered, or toggled. The park scene contains a fountain that summons a wardrobe from above when the user puts their hand in the water.

<table>
  <tr>
    <td style="text-align: center;">
      <img src="https://github.com/user-attachments/assets/66d33dc4-7264-4e4b-8ca5-894abd929d7e" style="height: 200px;">
      <br>
      <em>Figure 5. Elevator</em>
    </td>
    <td style="text-align: center;">
      <img src="https://github.com/user-attachments/assets/380fda56-4693-4c44-a388-b2b5fdfc8a2d" style="height: 200px;">
      <br>
      <em>Figure 6. Wardrobe</em>
    </td>
  </tr>
</table>

**Wardrobe**: To allow users to customize the agents, we designed a system to alter the appearance and behavior of each agent (e.g., Figure 6). Pressing either the fountain in the park scene or the button on the desk in the office scene moves objects in the scene out of the way, and the agents stop speaking. The users then grabs one of the eight outfits and drags it towards either agent to change their personality as well as their speaking behavior. This happens through a numeric system. Each outfit and agent appearance has a number component that will be used to match both components together. When the outfit touches an agent, the agent appearance currently selected will be set unactive and the one with the number component matching the outfit touching it will be set active. The text prompt will change based on its index in the array it's contained in. Then, the outfit returns to its original position on the wardrobe. The wardrobe can be sent away by pressing the button on it and the scene will revert to how it was before.

# Results and Analysis
![Results](https://github.com/user-attachments/assets/56bed391-1a48-46af-82e8-97c671a3a73f)
<br> _Figure 7. Reported Social Presence Values of Participants_

In our pilot study evaluating social presence, users rated four questions on a 1–7 Likert Scale (e.g., Figure 7). Results were generally positive: users felt included in conversations and comfortable in the virtual environment with the virtual agents. Users found the interaction somewhat realistic, but perceived agents less as real people.

# Conclusion
Our pilot study shows that users felt a positive social presence when interacting with two embodied conversational agents in VR. Though focused on two agents, this study lays the groundwork for exploring multiple agents engaging users in VR. Our findings suggest that VR developers incorporating multiple agents can enhance users' sense of social presence.

# References
Josyula, V., Mecheri-Senthil, S., Khawaja, A., Garcia, J. M., Bhardwaj, A., Pratap, A., \& Kim, J. R. (2024). Virtual streamer with conversational and tactile interaction. 2024 IEEE Conference on Virtual Reality and 3D User Interfaces Abstracts and Workshops (VRW), 1072–1073. IEEE.

Qazi, M. H., \& Qazi, M. P. (2023). Introducing VAIFU: A virtual agent for introducing and familiarizing users in VR. _2023 4th International Conference on Computing, Mathematics and Engineering Technologies (iCoMET)_, 1–6. IEEE.
    
Riches, S., Elghany, S., Garety, P., Rus-Calafell, M., \& Valmaggia, L. (2019). Factors affecting sense of presence in a virtual reality social environment: A qualitative study. _Cyberpsychology, Behavior and Social Networking_, _22_(4), 288–292. doi:10.1089/cyber.2018.0128
