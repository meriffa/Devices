import diffusers
import torch


# Diffusion models
class Diffusion:

    # No watermark class (applies to 'stable-diffusion-xl-base-1.0' only)
    class NoWatermark:

        # Apply image watermark
        def apply_watermark(self, img):
            return img

    # Initialization
    def __init__(self):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Device: {self.device.upper()}")

    # Stable Diffusion (https://huggingface.co/CompVis/stable-diffusion-v1-4)
    def StableDiffusion(self, prompt, modelName="CompVis/stable-diffusion-v1-4"):
        pipeline = diffusers.DiffusionPipeline.from_pretrained(modelName)
        pipeline.watermark = Diffusion.NoWatermark()
        pipeline.to(self.device)
        image = pipeline(prompt).images[0]
        image.show()
