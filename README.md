# VR-Dialogues: Navigating Conversations with Virtual Agents
_By Alen Jo, Evan Olson, Hrishikesh Srirangam, Aastha Sharma, Yadiel Valentin Rios, Abbas Khawaja_

(INSERT POSTER HERE)

# Introduction
Virtual Reality (VR) has significantly changed how users engage with digital spaces. However, many existing systems focus on one-on-one conversations, failing to address the complexity of multi-agent interactions (Josyula et al., 2024). We introduce VR Dialogues, an immersive VR conversational system that generates realistic, dynamic interactions between a user and two virtual agents. By integrating an LLM, lifelike avatars, and immersive environments, VR Dialogues enhances the virtual reality experience, offering a more collaborative and synergistic form of interaction. This system brings a new level of immersion and interactivity to VR environments, demonstrating the potential for scalable, multi-agent conversations.

# Architecture
(INSERT ARCHITECTURE DIAGRAM HERE)

The design goal embodies user comfort and control with a seamless user experience and a robust backend. This comfort is what allows the user to enjoy their experience and stimulates more intrigue. To begin, the user enters the game scene facing the agents. The agents will greet the user and start the conversation (e.g., Figure 1). A Speech-To-Text model will process the user’s responses, which feeds the text into an OpenAI Large Language Model (LLM) to generate the agent response. The LLM has multiple custom prompts that allow users to enjoy multiple personalities. In conjunction with these responses, the animations employ advanced lip-syncing techniques to make these agents more lifelike.

# Components
**Response Generation**: We use GPT-3.5 to generate contextually relevant responses based on user input. In our VR environment, the avatars "Kirtana" and "Ezio" engage in a three-way dialogue with the user, managed by a Python script that syncs with Unity for real-time interaction (e.g., Figure 2). The avatars also have interchangeable personality traits for an adaptive and engaging experience.

**Avatars and Animations**: Our agents’ avatars were designed with Ready Player Me and animated using Mixamo and custom Blender animations (e.g., Figure 3 \& 4). This approach enhances avatar liveliness, promoting user social presence (Qazi \& Qazi, 2023). Animations and expressions are dynamically adjusted based on speech sentiment, enabling natural interactions.

(INSERT AGENT IMAGES HERE)

**Environment**: To suit various preferences, we included a park environment in addition to a more enclosed office environment (Riches et al., 2019). We implemented an elevator system to seamlessly transition between these environments (e.g., Figure 5). This is accomplished with buttons that are activated with the user’s hands. The elevator is teleported to the corresponding environment, along with the user, which hides any jarring transitions from the user.

(INSERT ENVIRONMENT IMAGES HERE)

**Wardrobe**: To allow users to customize the agents, we designed a system to alter the appearance and behavior of each agent (e.g., Figure 6). Pressing a trigger moves objects in the scene out of the way, and the agents will pause in place. When an outfit touches an agent, their appearance and speech behavior will change. Pressing a button on the wardrobe reverts the scene to its default state.

# Results and Analysis

(INSERT RESULTS HERE)

In our pilot study evaluating social presence, users rated four questions on a 1–7 Likert Scale (e.g., Figure 7). Results were generally positive: users felt included in conversations and comfortable in the virtual environment with the virtual agents. Users found the interaction somewhat realistic, but perceived agents less as real people.

# Conclusion
Our pilot study shows that users felt a positive social presence when interacting with two embodied conversational agents in VR. Though focused on two agents, this study lays the groundwork for exploring multiple agents engaging users in VR. Our findings suggest that VR developers incorporating multiple agents can enhance users' sense of social presence.

# References
Josyula, V., Mecheri-Senthil, S., Khawaja, A., Garcia, J. M., Bhardwaj, A., Pratap, A., \& Kim, J. R. (2024). Virtual streamer with conversational and tactile interaction. 2024 IEEE Conference on Virtual Reality and 3D User Interfaces Abstracts and Workshops (VRW), 1072–1073. IEEE.

Qazi, M. H., \& Qazi, M. P. (2023). Introducing VAIFU: A virtual agent for introducing and familiarizing users in VR. _2023 4th International Conference on Computing, Mathematics and Engineering Technologies (iCoMET)_, 1–6. IEEE.
    
Riches, S., Elghany, S., Garety, P., Rus-Calafell, M., \& Valmaggia, L. (2019). Factors affecting sense of presence in a virtual reality social environment: A qualitative study. _Cyberpsychology, Behavior and Social Networking_, _22_(4), 288–292. doi:10.1089/cyber.2018.0128
