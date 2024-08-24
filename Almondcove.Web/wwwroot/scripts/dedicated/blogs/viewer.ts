import { acClearToasts, acInit, acPostData, InitLightGallery } from "../../global.js";

const ipBlogId = document.getElementById('blogId') as HTMLInputElement;

const btnLike = document.getElementById('likeBtn') as HTMLButtonElement;

const spLikeCount = document.getElementById('likesCount') as HTMLSpanElement;

const likeIcon = document.getElementById('likeIcon') as HTMLElement;

acInit([
    //async () => btnLike.addEventListener('click', await TriggerLike),
    //async () => GetLikeState(),
    async () => LightGalleryInit(),
    async () => setTimeout(InitLightGallery, 1000)
    
]);

async function LightGalleryInit() {
    const images = document.querySelectorAll<HTMLImageElement>("img");

    images.forEach((img) => {

        const galleryDiv = document.createElement("div");
        galleryDiv.classList.add("gallery");

        img.classList.add("gallery-item");
        img.style.cursor = "pointer";
        img.parentNode?.insertBefore(galleryDiv, img);
        galleryDiv.appendChild(img);

        const nextSibling = img.nextElementSibling;
        if (nextSibling && nextSibling.tagName.toLowerCase() === 'p') {
            const caption = nextSibling.innerHTML;
            img.setAttribute('data-sub-html', caption);
        }
    });

}

async function GetLikeState() {

    const likeRequestData: any = {
        PostId: ipBlogId.value
    };
    const resp: APIResponse = await acPostData('/api/blog/likestate', likeRequestData);

    console.log(resp.data);
    if (resp.data) {

        spLikeCount.innerText = resp.data.likesCount;
        if (resp.data.isLiked == true) {
            likeIcon.classList.remove('ai-heart');
            likeIcon.classList.add('ai-heart-filled');
        }
        else {
            likeIcon.classList.remove('ai-heart-filled');
            likeIcon.classList.add('ai-heart');
        }
    }
    

}

async function TriggerLike() {
    acClearToasts();
    
    const formData: any = {
        PostId: ipBlogId.value
    };

    const resp: APIResponse = await acPostData('/api/blog/liketoggle', formData);
    
    if (resp.status == 200) {
        await GetLikeState();
    }
    if (resp.status == 401) {
        await GetLikeState();
    }

}