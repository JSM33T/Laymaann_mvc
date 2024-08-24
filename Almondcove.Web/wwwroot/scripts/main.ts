import { acClearToasts, acFormHandler, acGetData, acInit, acPostData, acToastServer, fetchJsonFile, getUrl, Url } from './global.js'
declare const bootstrap: any;
declare const axios: { get: (arg0: string) => Promise<{ data: string | any[]; }>; }

const feedbackForm = document.getElementById('frmFeedback') as HTMLFormElement;


const lsearch = document.getElementById('global_search') as HTMLInputElement;
const ipFeedback = document.getElementById('ipFeedback') as HTMLTextAreaElement;
const btnFeedback = document.getElementById('btnSubmutFeedback') as HTMLButtonElement;
const btnShare = document.getElementById('btnShare') as HTMLButtonElement;
const url = document.getElementById('ipCurrentUrl') as HTMLInputElement;

const userCount = document.getElementById('userCount') as HTMLSpanElement;

var mdlShareElement = document.getElementById('shareModal');
var mdlShare = new bootstrap.Modal(mdlShareElement);


acInit([
    async () =>{
        if (feedbackForm) acFormHandler('frmFeedback', await SubmitFeedback) 
    },
    async () => {
        if (btnShare) btnShare.addEventListener('click', await shareIt)
    },
    async () => {
        const currentUrl = new Url();
        url.value = currentUrl.fullUrl;
    },
    async () => {
        loadUserCount();
    }
])

interface UpdateEntry {
    type: string;
    title: string;
    link: string;
}

interface FeedbackData {
    origin: string;
    message: string;
}

async function loadUserCount() {
    const resp: APIResponse = await acGetData('/api/stat/usercount');
    console.log(resp.data);
    userCount.innerHTML = resp.data;
}
async function SubmitFeedback() {
    btnFeedback.innerHTML = `<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true"></span> &nbsp Sending..`;
    acClearToasts();
   
    const formData: FeedbackData = {
        origin: getUrl().path,
        message: ipFeedback.value

    };
    console.log(formData);
    const resp: APIResponse = await acPostData('/api/message/sendfeedback', formData);
    if (resp.status == 200) {
        ipFeedback.value = "";
    }
    btnFeedback.innerHTML = `<i class="ai-send fs-lg me-2 ms-n1"></i> Submit`;
    acToastServer(resp);

}

async function shareIt() {
    const currentUrl = new Url();
    const linkholder = document.getElementById('link-placeholder') as HTMLElement;
    linkholder.innerHTML = currentUrl.fullUrl;

    const copyBtn = document.getElementById('copy-btn') as HTMLButtonElement;
    copyBtn.addEventListener('click', () => {
        navigator.clipboard.writeText(linkholder.innerHTML)
        copyBtn.innerHTML = '<i class="ai-check me-2 ms-n1"></i>Copied!'
        setTimeout(() => {
            copyBtn.innerHTML = '<i class="ai-copy me-2 ms-n1"></i>Copy'
            mdlShare.hide();
        }, 1000)
    })
}

async function livesearch() {

    const searchStat = document.getElementById('search_stat');
    const searchResults = document.getElementById('search_results');
    if (lsearch.value.length >= 2) {
        const response = await acGetData('/api/liversearch/all/' + lsearch.value);
        if (response.type == 'error') {
            return;
        }
        let sb = "";
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
            sb += `
            <div class="d-flex align-items-center border-bottom pb-4 mb-4 fade-in">
                <a class="position-relative d-inline-block flex-shrink-0 bg-secondary rounded-1" href="${response.data[i].url}">
                <img src="/assets/images/search_thumbs/${response.data[i].image}.svg" width="70" alt="Product" /></a>
                <div class="ps-3">
                    <h4 class="h6 mb-2"><a href="${response.data[i].url}">${response.data[i].title}</a></h4>
                    <span class="fs-sm text-muted ms-auto">${response.data[i].description}</span>'
                </div>
            </div>
            `;
        }
        searchResults!.innerHTML = sb.toString();
        searchStat!.innerHTML = 'Search';
    }
    else {
        searchResults!.innerHTML = '';
        searchStat!.innerHTML = 'Search';
    }

}