import os
import pygame
import random
import openai
import speech_recognition as sr
import sounddevice as sd
import wave
from google.cloud import texttospeech

# Set up OpenAI API key
openai.api_key = "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"

# Set up Google Cloud Text-to-Speech API key
os.environ["GOOGLE_API_KEY"] = "AIzaSyC125Gb92Jb5ImLGQGNyf9dIC3QJhXAKiY"

# Initialize Pygame mixer
pygame.mixer.init(frequency=22050, size=-16, channels=2)

# Conversation lists
conversation1 = []
conversation2 = []

# Create Google Cloud Text-to-Speech client
def create_tts_client(api_key: str):
    client_options = {"api_key": api_key}
    return texttospeech.TextToSpeechClient(client_options=client_options)

# Create the TTS client with the API key
tts_client = create_tts_client(os.environ["GOOGLE_API_KEY"])

def text_to_wav(voice_name: str, text: str, speaker_name: str, path=r"C:\Users\abbas\OneDrive\Documents\GitHub\VR-Dialogues\VR Dialogues\Assets\Resources"):
    language_code = "-".join(voice_name.split("-")[:2])
    text_input = texttospeech.SynthesisInput(text=text)
    voice_params = texttospeech.VoiceSelectionParams(
        language_code=language_code, name=voice_name
    )
    audio_config = texttospeech.AudioConfig(audio_encoding=texttospeech.AudioEncoding.LINEAR16)

    response = tts_client.synthesize_speech(
        input=text_input,
        voice=voice_params,
        audio_config=audio_config,
    )

    filename = os.path.join(path, f"{speaker_name}.wav")
    with open(filename, "wb") as out:
        out.write(response.audio_content)
        print(f'Generated speech saved to "{filename}"')

def playAudio():
    try:
        pygame.mixer.music.load("audio.wav")
        pygame.mixer.music.play()
        while pygame.mixer.music.get_busy():
            pygame.time.Clock().tick(10)
    except pygame.error as e:
        print(f"Error playing audio: {e}")

def gpt3(messages, model='gpt-3.5-turbo', temperature=0.9, max_tokens=100, frequency_penalty=2.0, presence_penalty=2.0):
    try:
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
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return "I'm having trouble responding right now."

def tts(response, character):
    if character == 1:
        print("Kitana:", response)
        text_to_wav("en-US-Standard-H", response, "girlAudio")  # Female voice for Kitana
        conversation1.append({'role': 'system', 'content': response})
        conversation2.append({'role': 'user', 'content': f'Kitana said "{response}"'})
        with open("sync.txt", "a") as sync_file:
            sync_file.write("a1\n")
    elif character == 2:
        print("Ezio:", response)
        text_to_wav("en-US-Standard-J", response, "boyAudio")  # Male voice for Ezio
        conversation1.append({'role': 'user', 'content': f'Ezio said "{response}"'})
        conversation2.append({'role': 'system', 'content': response})
        with open("sync.txt", "a") as sync_file:
            sync_file.write("a2\n")

def saveResponse(response):
    with open(os.path.join(os.path.dirname(__file__), "speaker.txt"), "w", encoding='utf-8') as file:
        file.write(response)

def openFile(filepath):
    with open(filepath, 'r') as file:
        return file.read().strip()

# Function to listen for speech
def listen_for_speech():
    recognizer = sr.Recognizer()
    with sr.Microphone() as source:
        print("Listening...")
        audio = recognizer.listen(source)
        try:
            return recognizer.recognize_google(audio)
        except sr.UnknownValueError:
            print("Sorry, I could not understand the audio.")
            return ""
        except sr.RequestError as e:
            print(f"Could not request results from Google Speech Recognition service; {e}")
            return ""

# Function to record audio
def record_audio(filename, duration):
    print("Recording...")
    audio_data = sd.rec(int(duration * 44100), samplerate=44100, channels=1, dtype='int16')
    sd.wait()  # Wait until the recording is finished
    with wave.open(filename, 'wb') as wf:
        wf.setnchannels(1)  # Mono
        wf.setsampwidth(2)  # 16 bits
        wf.setframerate(44100)
        wf.writeframes(audio_data.tobytes())
    print(f"Recorded audio saved to '{filename}'.")

# Load initial prompts
with open(os.path.join(os.path.dirname(__file__), "Chat1.txt"), 'r') as file:
    prompt_content = file.read().strip()
conversation1.append({'role': 'system', 'content': prompt_content})
conversation2.append({'role': 'system', 'content': prompt_content})

# Initial greetings
kWelcome = "What's Up! I'm Kitana!"
tts(kWelcome, 1)

eWelcome = "Hey! I'm Ezio!"
tts(eWelcome, 2)

# Sync file to check for 'b'
sync_file_path = "sync.txt"

try:
    while True:
        # Wait for 'b' to be written to the sync file
        while True:
            with open(sync_file_path, 'r') as sync_file:
                sync_content = sync_file.read().strip()
            if 'b' in sync_content:
                break

        # Clear the 'b' from the sync file
        with open(sync_file_path, 'w') as sync_file:
            sync_file.write(sync_content.replace('b', '', 1))

        # Listen for user input
        record_audio('user_audio.wav', 5)
        user_input = listen_for_speech()
        
        if user_input:
            print(f"You said: {user_input}")

            # Append user input to both conversations
            conversation1.append({'role': 'user', 'content': user_input})
            conversation2.append({'role': 'user', 'content': user_input})

            # Randomly select an agent to respond
            agentSelected = random.randint(1, 2)

            # Get the appropriate response from OpenAI
            response = (gpt3(conversation1) if agentSelected == 1 else gpt3(conversation2))

            # Respond using the TTS function
            tts(response, agentSelected)

            # Allow the agents to converse with each other, limited to 3 total exchanges
            agent_count = 0
            while agent_count < 3:
                agentSelected = 2 if agentSelected == 1 else 1  # Switch agent
                response = (gpt3(conversation1) if agentSelected == 1 else gpt3(conversation2))
                tts(response, agentSelected)
                agent_count += 1

                # After the agents finish their conversation, continue to wait for user input
except KeyboardInterrupt:
    print("Manually terminated conversation") 