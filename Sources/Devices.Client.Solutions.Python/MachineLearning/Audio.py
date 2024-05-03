import datasets
import torch
import transformers


# Audio models
class Audio:

    # Initialization
    def __init__(self):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Device: {self.device.upper()}")

    # Automatic Speech Recognition (https://huggingface.co/openai/whisper-large-v3)
    def AutomaticSpeechRecognition(self):
        torch_dtype = torch.float16 if torch.cuda.is_available() else torch.float32
        model = transformers.AutoModelForSpeechSeq2Seq.from_pretrained("openai/whisper-large-v3", torch_dtype=torch_dtype, use_safetensors=True)
        model.to(self.device)
        processor = transformers.AutoProcessor.from_pretrained("openai/whisper-large-v3")
        pipeline = transformers.pipeline(
            task="automatic-speech-recognition",
            model=model,
            tokenizer=processor.tokenizer,
            feature_extractor=processor.feature_extractor,
            max_new_tokens=128,
            chunk_length_s=30,
            batch_size=16,
            return_timestamps=True,
            torch_dtype=torch_dtype,
            device=self.device,
            generate_kwargs={"language": "english"},
        )
        dataset = datasets.load_dataset("distil-whisper/librispeech_long", "clean", split="validation")
        audio = dataset[0]["audio"]
        output = pipeline(audio)
        print(output["text"])
