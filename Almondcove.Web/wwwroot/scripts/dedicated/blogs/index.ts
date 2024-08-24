import { acClearToasts, acFormHandler, acGetData, acInit, acPostData, acToastServer } from "../../global.js";

const spBlog = document.getElementById('blogPlaceHolder') as HTMLInputElement;

acInit([
    async () => LoadBlog()
]);


interface BlogPost {

    blogPostId: string;
    title: string;
    slug: string;

}

async function LoadBlog() {

    const likeRequestData: any = {
        
    };
    const resp: APIResponse = await acPostData('/api/blog/getall', likeRequestData); 


    console.log(resp.data);

    resp.data.forEach(blog => {
        const span = document.createElement('span');
        span.innerHTML = `<a href="/blog/2023/${blog.slug}">${blog.title}</a>`;

        spBlog.appendChild(span);

        spBlog.appendChild(document.createElement('br'));
    });

}