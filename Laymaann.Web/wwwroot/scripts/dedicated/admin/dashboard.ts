import { acGetData, acInit } from "../../global.js";

const userCount = document.getElementById('user_count') as HTMLSpanElement;


acInit([
    //async () => btnLike.addEventListener('click', await TriggerLike),
    async () =>await loadUserCount()
   
]);

async function loadUserCount() {
    const resp: APIResponse = await acGetData('/api/stat/usercount');
    console.log(resp.data);
    userCount.innerHTML = resp.data;
}