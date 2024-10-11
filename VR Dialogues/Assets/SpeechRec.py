import speech_recognition as sr
import openai
import os
from gtts import gTTS
import pygame
import random

class MultiConversationAI:
    def __init__(self):
        self.conversation1 = []
        self.conversation2 = []
        self.api_key = os.getenv("OPENAI_API_KEY", "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")  # Replace with your actual key
        self.setup_openai()
        self.setup_audio()
        self.load_prompts()

    def setup_openai(self):
        openai.api_key = self.api_key
        openai.api_base = 'https://api.openai.com/v1'  # Correct base URL
        if not self.api_key:
            raise ValueError("Error: OpenAI API key not set. Please set the OPENAI_API_KEY environment variable.")

    def setup_audio(self):
        pygame.mixer.init()

    def get_audio(self):
        recognizer = sr.Recognizer()
        microphone = sr.Microphone()
        with microphone as source:
            recognizer.adjust_for_ambient_noise(source)
            try:
                audio = recognizer.listen(source)
                return recognizer.recognize_google(audio)
            except sr.UnknownValueError:
                print("Could not understand audio.")
                return ""
            except sr.RequestError as e:
                print(f"Could not request results from Google Speech Recognition service; {e}")
                return ""

    def load_prompts(self):
        try:
            self.conversation1.append({'role': 'system', 'content': self.open_file("C:\\Users\\Student\\Documents\\GitHub\\VR-Dialogues\\VR Dialogues\\Assets\\Chat1.txt")})
            self.conversation2.append({'role': 'system', 'content': self.open_file("C:\\Users\\Student\\Documents\\GitHub\\VR-Dialogues\\VR Dialogues\\Assets\\Chat2.txt")})
        except FileNotFoundError as e:
            print(f"Error loading prompts: {e}")
            exit(1)

    def open_file(self, filepath):
        with open(filepath, 'r', encoding='utf-8') as infile:
            return infile.read()

    def gpt3_agent(self, messages, model='gpt-3.5-turbo', temperature=0.9, max_tokens=100, frequency_penalty=2.0, presence_penalty=2.0):
        try:
            response = openai.ChatCompletion.create(
                model=model,
                messages=messages,
                temperature=temperature,
                max_tokens=max_tokens,
                frequency_penalty=frequency_penalty,
                presence_penalty=presence_penalty
            )
            return response['choices'][0]['message']['content'].strip()
        except openai.error.InvalidRequestError as e:
            print(f"OpenAI API error: {e}")
            return None
        except Exception as e:
            print(f"An error occurred: {e}")
            return None

    def tts_agent(self, response, lang='en'):
        """
        Convert text response to speech and play it using pygame.
        """
        try:
            tts = gTTS(text=response, lang=lang)  # Generate speech
            tts.save("response.mp3")              # Save speech to an mp3 file
            pygame.mixer.music.load("response.mp3")  # Load the mp3 file
            pygame.mixer.music.play()               # Play the loaded audio
            while pygame.mixer.music.get_busy():    # Wait until the audio is finished
                pygame.time.Clock().tick(10)
        except Exception as e:
            print(f"Error in TTS: {e}")

    def save_response(self, response):
        """Save the response to speaker.txt."""
        with open("speaker.txt", "w", encoding='utf-8') as file:
            file.write(response + "\n")  # Append the response followed by a newline

    def write_sync_file(self):
        """Write the character 'a' to sync.txt."""
        with open("sync.txt", "w", encoding='utf-8') as file:
            file.write('a')  # Overwrite sync.txt with 'a'

    def start(self):
        print("Welcome to the Multi-Conversation AI! Just start talking when you're ready. Say 'stop' to end the conversation.")
        agent_selected = random.randint(1, 2)
        
        # Write to sync.txt each time start is called
        
        
        while True:
            user_input = self.get_audio()
            if user_input.lower() == "stop":
                if self.confirm_exit():
                    break
                else:
                    continue
            print("User:", user_input)
            self.conversation1.append({'role': 'user', 'content': user_input})
            self.conversation2.append({'role': 'user', 'content': user_input})

            response = self.gpt3_agent(self.conversation1 if agent_selected == 1 else self.conversation2)
            print(f"{'Firebolt' if agent_selected == 1 else 'English'}:", response)

            # Save the response to speaker.txt
            self.save_response(response)
            self.write_sync_file()
            #self.tts_agent(response, lang='en-GB' if agent_selected == 2 else 'en')
            agent_selected = 2 if agent_selected == 1 else 1  # Switch agents

    def confirm_exit(self):
        print("Are you sure you want to exit? (yes/no)")
        response = self.get_audio()
        return response.lower() == "yes"


if __name__ == "__main__":
    ai = MultiConversationAI()
    ai.start()
