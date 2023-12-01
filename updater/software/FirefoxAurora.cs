﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "121.0b6";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "72f2e3b67f2d9b319ee83503c9ee41fbfee2efcad30896323e08032c3c97d59e4a85199ebcff2569f2ef8016b72006f60aa726a8bb6d7803c5fc1ea0dea12e51" },
                { "af", "eba06ca5e15087a6611fd6f7ddf14297f4d0091787c274ab89012a64ba505d1dba0d48c7a10c534f2b97b29cc5807bf4037e69dc1f4b791b30e86beafd4b5d7d" },
                { "an", "b0490c90f278f63e10fb8ca49653a6219e09860086f6d16fa14069d4110f48cbb14448b96cba68a11c06498c4e456d2bd30befbbad3e522eef99152e75450a3e" },
                { "ar", "e5b8c34f1542ffc526c819bdeea40ab24520a2b6008a2b43ee550787fea0fd49de865dfda7b791a99d79b5e4494286fd911812150e2dc96d124cb747f5387d59" },
                { "ast", "e9d4a015dc524cc1af4d827719e0a8c73aef70a51d7867cbda4cb65dd15b94ebbc3e5c3bc64161686200377b2654cdf8c79cad2c1626dfcd1a79d9e150057ae6" },
                { "az", "606346962af33669d6bc5c13bbafb35d6d9bde87dfbbac057fc086d1c082c89f18ea1229d5abef8e40f08ccfea21ad48e6f8f65c46afcab531fac45fbf358c62" },
                { "be", "1f716bd6017e6cd81642d7c97a2643c02198b1a3c45aa64e385361e46787a71b323fb7eab09bbe4a8e2443049b76a52066b1ad08a35cbc9a73ac215f606baba7" },
                { "bg", "eca2d8dbe61ff5b27c1ab96bf70829c1bd11e033e12fc5b24cfbd8387289128054514f706331ef3ea74d46b019c3da409fdbedd1bbbd7d916c60283875d93a27" },
                { "bn", "5922402c32a38278d7ee6f43aa5d62259d6a6e6481ef4030a0516f529f61d7c58a1b684b78af9f83839ce8492036fb37618ffee68d90bf6c7f5fec24fc6f6bb3" },
                { "br", "7c00494261e5a6fb2914004662948c2754d78cb18daae3fa51adf0b1572995ae126e8d7817b0ed5c5147e542ff8f83379c0568a2c29c32a99257315a8733e09f" },
                { "bs", "8989bcca34bb30d5878e4805b03c4551254ef67d4324637d5e623ab5928b06ef088d4c26600d3d166b2ab8e90a46587dfa83b8db573854a050712ab8a9ab1ec7" },
                { "ca", "743c615ee469afb84b9229172490fda84cf367d014c8dd23a3948edd50214e596b77573b2a8748a12a280f4d042362d5a7e9a8804b24bfc2d6993ea03ba25a18" },
                { "cak", "15247b6c1b95b66b953a8fe04e0625e6b9a5205483d0ae90c7a19ca3bca3dddfc7285c20c256d98dc6828bc936626204c765fcd535b8435072bdb51afc9bf8f8" },
                { "cs", "8b4d3729565d4637c7ee8ff0a8e1be3a9f64028de7bab7c220e2fa4bcb2719fd71873f087d86d674ae39614237693e37ed0a07fe913f942920bddd02788a616d" },
                { "cy", "b5fb6f573ef5d47bd74decf1f4392e17dd7cec90202281b17f354bdf6bf0de663fdf1b19aeea3b9ca3055bd35e5912e05937d062ecb6c16229a4e69b2b3fed9c" },
                { "da", "c14a43773a44a948431454a87a6645f9ea5b1025051850978ac2d2523e4760741ab8f2671ee51ca488605f77d1cf967eae646d265073e9f24292fb80a79b4067" },
                { "de", "445280b7590d13c68f7fdd4fb7f8d715d047cdc5774adc6c0b350bd6db0c74258bb3323953465058e175ec033937452aaa5add0e0bd85bcb2b085ec10c70270a" },
                { "dsb", "7879015e11b765d12f77ebb094a95d84b04e68ac7ccbaabe4a459c77d5bfb284d58531114d53a61473ab70299a373eca7b9fe91ec58ac8655e3b289a64b05b20" },
                { "el", "43958c4f9b11e1523f4e5f9466831aad695bb9ab8fd8c761668e8e8604492203661702bdc4432b672cfac7df1a4033acfdfa730d1cc6572fa7d47d88d247abf9" },
                { "en-CA", "53031dfb4ac511eb6f2eaa3d46b632375484be7a9e569fa8e424dbacd6f8d083bee696b5ae8d1b9a1a37a5cc4358731972e911ed9390dd02c505e70fbd1a2021" },
                { "en-GB", "3628595c981f8dbaded6f9f5a105a062eee3a4d486143d313146e1e3ef2a70fecb7a6a4772465c1ed816dac3fda6536d0460b8641e1614816ff4ffafcccd75fc" },
                { "en-US", "8a4c3dd72724a57a9a3d41c95b0db70b0c4e465dc1f9daad5abd9345b558fd0d722d31bbac195ec48c5953bc5d67d631aa847111b71144152d31ae84082b09a8" },
                { "eo", "92c233f89e63207718964732550b8f7b337a968a5a1a058e4b042e12feff7793ebb42f4493b442e845f2b0e404e6e37042785bac07b5af7c9a8c47d4b83fe11b" },
                { "es-AR", "0bd3995458cf1c84af58167604a7d7b2d289e0aaea1edd472313e01a5ffe45255531d50bddf736a34bfe7c6568bd06702d535415b52845aaf159a0a1767968a0" },
                { "es-CL", "c03a7b53a148771de2bf266f2db82ec811bbcc4ae33e448569911a4f144dd02b4f4d810053ac8aca0267663662fd5b627914b2044f4cc8857792990a7daf617e" },
                { "es-ES", "b06089ab7a969dc102965184e23f528861e3709321cbe07c7e159bbaa3f58a3a567cc63ef61a643a6c79bd794e859adabba5b674b9936c0be80fd56129042b56" },
                { "es-MX", "6af4ea9635803d36f2e466463d4c29691125e01bb6563ed04abe9db124807ec49218b8ccdae79097386dfc49bd309f9ab5c6438fa3bd5ee92af41a1381757f7a" },
                { "et", "71f045d00156ec93e6f9b13226ca775d9bc695e46ad60b09119da710278aeff64c4a01005db2c08c6c2dc71e644ddbfbd0f068691addd62a645ef57c659ce5db" },
                { "eu", "e1b266cd3e58e9d3841fbb3531400e8c3a9c7c150d419512974705c5277fde667416e25f4ed82eb69c370398b5ff67cb4e4779dbac19c848ddeb3cc943695360" },
                { "fa", "b423003cf385e18c08209dd9edda91d5b3ba7259b8c388515d91966458bcd74d0ce89477df5a2a59e916aafce3bd954f9fa823a0cad1560e3145975f82a133c2" },
                { "ff", "0288e21a240b59709fe9c34714059bdb2eaf1a589d0ee7f81c08d662c2d51299a42f3b0f4f8dcea2508af262fd2121d554f5e5b3e75cc9e180b1c8dc33a15868" },
                { "fi", "633593478ee802505058cf6aa72a0fb90d18e82f1f8ab16032acef19eedea593da90b23c0d0f45cdbc00e67b2f620d3d433811c30ba438d2bf69f77bcd14ad8e" },
                { "fr", "e36b67d01aa7cc2db2935c087e31dcbe7ae1b1fa6db50a5cb1a1871d7a0d283923bed26f19c89f0d2bab906649887a6fdded326818e465cab58299e9e14fa061" },
                { "fur", "b98f0ca830a7cfc3473b2b3eadf71d62f3f0689976f8eb237bfad087d48539e83c68962611421e0c96b591198546c03c9763fcba48a02e6c40938983fba13f0d" },
                { "fy-NL", "625b2eaed2c983820a8bcdcff3a83199159d751b4bff57986691c217ede8ba7ceff60b2d0dee0fa90fba84a45e4d8b207dd44cb02c98da64cb8efd6e756bfceb" },
                { "ga-IE", "be9e491a53e7e72153f3562d0193fda692c24aae824ec090cb09bc9abf8a53534a08ec747d713d98cb14e0c67d6c35ca7f1dff669c1b04409730d2ca4b1e7b20" },
                { "gd", "1cb0d7feb9a01bba1734ef540efde77758c884ab72ff3c585c214cdb3ffb6df9c4096ba1b2329438cbe847a57a78c98e00255cb7b9ce2d1c37823451a0a76991" },
                { "gl", "59de99f44067073393a3d8d7aa1c560729cc56e3577809d19ce7343a039aa6dd139cfe763a4a6430efa0e266bb39433ece6e7963c4757b7ec93d3e815b493b5c" },
                { "gn", "e2a6799cc768489c2a4048be8536ea21d7627b3608cf2f00f5328fbff3359a72e7bae8c01a07bbe79435c8d6aaafedbf41ba6bec26ba35ff00e11b3b157b3b63" },
                { "gu-IN", "c5d125811188c6b14f2a30f04c4f98b67e28555c410297ebccfdad1c71b40f1559807f482e74c9c7d8077e2c83572bb12f348b4eaa882f2868b3779aca25675e" },
                { "he", "e9a67bcd716c9e855e4e5e9894a0a0452da8bdad6e0efb9c52d6d865646e5df5ce9240789b5187094b0ba99a211965439f940a847d6637c92f55656097a3876e" },
                { "hi-IN", "ccfa62cfc7424ab674629d015d297e922f4e3cee1021f511924b1f60d4b63ba9908e5209caa207b846695aef5ef08ae3274b561c2ee94dd35852ac1d556c7f73" },
                { "hr", "a895a1fa61737149a383313d5a0dc3d4e0e516ad62c4c8980bafe597bbd6db582b26c5ba8c8cfb28e3ce56c223e66d3fb2ff802620dd4120513a4adaf8299675" },
                { "hsb", "78b1c34f4b49409b6ca5ad875ed95900483f0627e545e1f4eaec138fcb5f81957dbe81fb32da923cc89ff109690ae2a580c81efe40d4ec92443cb59c9cf9f35c" },
                { "hu", "69b032d3a7d820ce7acec5c69da9e1ff16278a6d3702d67c74a0d8638d95130d4f47266d847bd515cf0b3d628c3ae9ecf9a782a74fa1bc343b5a7d5eac64c828" },
                { "hy-AM", "0b8a58a8601a0a2e344275d2743925c2d1f4e6fe9f691475ae1db532407efc0bfd7b1d7801bf3ac8c6e0c4e81fd0e986c9d2fc9f3384a2f24bb9b0d2f90e8da9" },
                { "ia", "8d39cecbcfa047dc6a3879d2accfd7acfb46c8eeec4a0a9edd37ba03cb5f42e4de7587c99ea3b6c1a59004126c920dcb4d3043e04951d90bee02b233da00cdec" },
                { "id", "f3c483744052092334ea89bab12a7226176d93f0ededd476ea28063a81a545f6a9452dd17f92e766e0c5567f2fbc04de99eb4ab84c0c52d2c7fd7ac57c5f1f79" },
                { "is", "15daf1174320fb871ad906fbbf3491309a183172778319ff5adc217cee395c16ef96221cf990fcaced65827e3e9814cf795f7a6e089a7b67faea94728358e65f" },
                { "it", "92ce81c71a82a8a3601f3441f910bf4e506af5803276f585968eb94319e184a02ebc3945a9a26e844e68b882893354b6c1fde712f2a840ae7d6c11ea0dc80c07" },
                { "ja", "b6a34d3f68eadd240a08815c300054b4726acd7528e31b4db70eaa7ae8f4c1cac4a9f76c394f58bd708be34e559bde735223d28932e57d69cf9e32ee56d688af" },
                { "ka", "15c4cf12563ec9db94daa19d817c6d9def6018b529224066d4cae75649c2271f1dd0e3e131c2050272d729595dcc8bdaf54e7f4b76264682f41f57d745ab7c4d" },
                { "kab", "3b1a706d2117653a746e8ec213f4cd36a7b2fb697eebcc5530cee83ad709799b28335a281b3bfc18b7f5416945fa8d7863e43774bfcdb4dee8be5253d1d20e6e" },
                { "kk", "b9dd6522d5363923c1802ab14962c210380e4ba48dc4ccd69931cd08ab214f31194eabe3c527c24bd4f408f015fbcd81481b95e332d0df4f3bba3c1398e77362" },
                { "km", "0ec80ebf1a9acad8df08410416ca28578d9ce35a5cd9185c065847ef7efaced59a82469066296afc29f08df863e9dbdcbe1d42e49c0cb59ae2ffd231446585ec" },
                { "kn", "04e31baf79fb169898252fba0e6c1ee62b742dd898eabc49829c39e2ac4f4214b2e3ac0b863fc1017f67f91b1f6c9c85aef39d88f5d0d36b0d658ced47d773d6" },
                { "ko", "3d0e5265486a5cde0ddc0056194692ff41a780273cb548dd5bb68e4d46564c8f23ee1b57b9eff1ad40ae16dd1b4c30ba7c74e9e9c082ac5eee9024da8b253092" },
                { "lij", "d210aa739e67c5d0fd16f9b9b3b7df74ecaade31543bae5014c38d19020ba2d2ae1d197804c543b8c242f53a1f66bcbc35d36ea9e5af0561e441de1ebf8941e2" },
                { "lt", "13ebed7e3ad80df671695673f9ed38c58e9332067b7ddd7483e789270f20ca266b3c6cbf38e58c49b02c5bea85aa869afd4ff65c1d2f5282aeddb40df0defbae" },
                { "lv", "0779f7f853ba112205e27d4225b4fa429a7fc62db247de4df1811445f5cfeacba2e6eb24c00258520e45f6db251566b3dacdc35648be4bd532361356f6f3658f" },
                { "mk", "b92cfcbd97bf94171eb9a5f28bc332b5a0833787d02f2738b4e6a839c913b65b531a343bc81e39a25d4df97bbf4bb853a9af18c87af10e262145c95034da8149" },
                { "mr", "f2055a2b236e2a7b1c5489d79c22d7e419f1265bbaaa351020005cb59405731c7348d5ee20849a9f11f927376e1ae32d11a86eceb7528598c7b06ad78762e8f6" },
                { "ms", "c3ea8a3a420b6ffcd173083fca66d2c196a0a9f2da7847851cbc2e65bd92a90620480d2bf6c3d710aac6bba1b7670f565aca8c50518e96aedf6ea28f8cf6ef6f" },
                { "my", "8fb0154668ce6a545e1327d9b9fd1bad13cf601a9183c2a40955a9e07dac69e3190a60561836ea266175d34df7d3ee4edc0c872b3113ec4105652e930e9e9f9a" },
                { "nb-NO", "e4f42a3e883b8a0501e57a73859d30970c8592c5eaab818f9533dba182df2e8f8bb8c07e92f773872e4884f812d4ba02a9a80c280c279843f47c21468d257bda" },
                { "ne-NP", "4e179d25062c4f1561295b43667d86824504e22c25879834697e81792f573d716618c68ebd2bb18734d48d09c7440f7bbe83fecfe3a0b33f2b4a594759c4b8fc" },
                { "nl", "e16c245277f84869a6c88d802a25dc163608b94d8337a4f61e45cdf002b3e6a5136061ff326e2f8c95b5ac26691535d3f7bab78e499301d39d8cba99b5bf616b" },
                { "nn-NO", "dcc683443b7db8945a64023acbfbc3447eb7a27955e72459bd1dba153dffe7c5c480235a3e0f5da0e77ca5d371240921d2d3625029ac2bd48dfded8b78dc326c" },
                { "oc", "9e84a05b6dd8497b17212672b167693baf617bd8b8299dc732fff3d454843579d23dca7cc52510e7b6d04efb2b8b6804924197d1965f102ab0dea4eda9062d53" },
                { "pa-IN", "52a96ad84b46c1f479a811266b094655872c4ba28cfbf4116bc1a78dbe0b5585b0547e2be4f7e908fcd54f33460d7cf116c6c7e45a0bdb5f39cd08c66d0aab76" },
                { "pl", "21554981073205d71f46ca513e6db6a5bb719460ffc5e91145319a1b7a3f4c08b54049c91221ffb5b28b04d6c0d06fcb976a2bb445c16b6fb254072181cc2832" },
                { "pt-BR", "b7b37965532f77c4c02c2afd4294776e7ffd9cc14a5f0b9974cd12e4d430beac35531e43032ccbb5e3b871c1a0d2f9e303ecb1fdf6e2eb09e432f5798cf282e3" },
                { "pt-PT", "e1e3de6aea50c151fd190bba87e2560772a1f145bfb7483f106f895deb37e4aa4fadf977d4efab5c0127bd127627cf3b5f6fadc812ffbb4524cf44aa88854417" },
                { "rm", "e63c240576cba6045a615e95cc8568a2b009fc0abe993f997319cd7f1cce8ac8f1d7c5169412622fc103a5c7bf8d833c6b4485e57041a30b1c308e3824d4c6be" },
                { "ro", "692507912a94f827fe295fdc6919fc34833bb00edc51e2c93ff0ae0abab62a35f4388e28e4947432fa160136f8c998a1bf36ca4ae253f91f289a89573115db67" },
                { "ru", "c5b1b8032e5c1d3c3222aab7f2db0bf0b78c1a862c0b16cad586e9adf98c618ea0b7a901cac592cf0c4559baae90a4099b7c4468160544f6fa546b303e89b69b" },
                { "sat", "b0229b2182b795389f35cf99b91994ff913f4531703002afff7b2499704c516db88a4379f773f6b8311650e1a0611fe6147b53f9554ed3f0f3e32da2d7e7df57" },
                { "sc", "18b204c4a77509295d9882c2cf531e1c4fb3c0ae41a759723a23b0b9d538a0e25a89f941c6975d1ed1adbae6a6a0c35c2d30d3d52a851290d2fb3acfc68098da" },
                { "sco", "a502533d62fe84889cfd52ff290e802827a1bb398a562b9173d5baef732cb36adc6c5179ae1891e7b5a0878666427e845fabcc26e99d4cdad483746455c6f009" },
                { "si", "b120f9114dce412c602dc49ebeb27e771443c0a5b61feced3d22f9bd99f49e7a4e177029a9d443069dc5df5bd03c6f20659fc186e72393f2eb262ba0efc112d8" },
                { "sk", "9df45b1f32bbab34a5e6eda5d96f017b368740e17787cf1e0db5e7dfd929635a463b1584c2af9a388eb03f2f5f214cbfa055df6f255a048aa3414d673f767b1f" },
                { "sl", "39eeada662fe929510bb78ebcc558f64e97c343c71ccb26d1cba6a511aebed80a26a0e6303517fdfde6dc394237cc9eb08f3a370f6e03c101739b53fccd237b5" },
                { "son", "aa37acdc6075be601a2242e8f17ee5c040004f20a2a7b493ebddd67110eeeb321dfe6a6edfbdf6d470a644f619b5eb7b9ff0e96357834e15e7595a38b39277dc" },
                { "sq", "d924fd8ff1e54e9fe7153b9ad8b8020d03e51f9be12c33196d1f486aa98d105e4aa1ce5c585f8add4f32f69e282c2054b2f0129c6cd1d5a945a2a191a0ca30fb" },
                { "sr", "b97bcc88409f4a8658561df3d068f33ab0a00112cfa1386e9d15a47001226138135896584bc0a1d251eb22eba76f57bceef53dae2bffc1abe89f7170b47a4143" },
                { "sv-SE", "be6795e2a5917e7019dd7a390b5f679603794f08cd025b098c5c8b7f73bbe110fbfdcfdf6277a0fe903c99c9d20c7b32ab78e7a1d5e13a1be4f5323ffbec157f" },
                { "szl", "85b615433489eb5815d1b311b5767be3f37c5d5fe15e6b3ef23ed857c2b9b7f724d2876eb6de7270becc737b68f2c6ecfa26c0c77e1c4df5f521d46ce1f8b9d4" },
                { "ta", "84fb8cc368b01f5a6949bcf8b0f92b77068521cf5fbc084eff27974e14c8ee7efd2b3ae1e63cf30f8301e867687cab70ed2c1e1b838a94bd493087dedbb282ab" },
                { "te", "9a1f7290687940db03d7448f316c935afdf4c58c1f80a8750fac9509a7d320bf96efa566862238440e9cf7687919165b424c5746076a9d66b39355afee98c930" },
                { "tg", "b044781aa8d171d02510a4685e7c6f60d05808b496a77b4d1bfb4f8a37518d4656c2c5f211e68ff9492d1e5a5202265582ad974f1eab241728da8fc43ef28279" },
                { "th", "304a5a80479103800569581fb5cbab234a7d6ca569e10d4df333dd8f2d577367512cc033f9fb35274cef860f0fd23ddee4e2e41bb276533e5fe48939edf6a1b3" },
                { "tl", "b44744b2fe770d2d981c50fbb804d606609bb6753e6bf44f4d21973f6f5e179bf5e28bd07cdf55f5db27d0714fcd5b7a28cded1d0b6305cc551a1f92a6655772" },
                { "tr", "454c32e79b2d141e9888e691bba35b3b610f94b0cd9df04b3fa0c6a6000d4c5cf9fc30643a1b915f9c6dae2fe9b878ebb69078c259c3ab41678ccc600425cdc3" },
                { "trs", "fab02df3dbed2e2451581b133238a8a9bde2963f1ca820fe3512b4a28097f0441a7b224636481e5ac1dafdfcd73822462f29211ea7374a90ce8a27e7797e6120" },
                { "uk", "4aba82feb8ef9bbb051cee29af08dde946c051998a7b3226564643b0927f7bbb3fc79c26cc1067421ac14de9c8e50f59d7b6d74c557aa90b2cacc9406eb6e194" },
                { "ur", "7bc50ee5c39065667c8570d5f9e77a54ead6bd72d3873105ec44e2b3540c655ade2c5452e7cf24b56065411c1b1e7f63c974396ca3cfb95fd1773a855b56926f" },
                { "uz", "043bfade4577181eecfb4743a4a41fad03eedf071340d92b875933183d5fcb30b7dbff018863b708452aec14b454457c41fe2a318e69b5ee77350db56bba4d2a" },
                { "vi", "da3dd7ef01d2fb7687de421ed4a0b5c6d0e140d67d48bb700c30758618d6c339bf5e3f7366d5226dc1168c9bf001f718cdeb23ab67c7a17d7d6d52cf2284f903" },
                { "xh", "1b87b462e18744650c3b939d5de6244467f8c8485da537cb950f5342c36905ea75a26c3ced385d0ed8dddb296d877aa92410881e543de13404619f9f25eeaeb0" },
                { "zh-CN", "61fcc0be913582af8d1af811bff60a6f320c2e82bd87e646a1fd73093927b50bfe41386cd08fbf7e4969989db724a6c32f83d05e3b3c918e7464858d82a15e60" },
                { "zh-TW", "5808887b3814c74cba88261936b029452f0656a13379431a42824866fe3a50f960aa98fb6f792ca05532dfafa66616717b6f6656815640c278f90cdcc0ef01c5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6a880504426136e2b847396a51a19ab9791f87d355c5e608f7bc9dc5053bb796bc6d002239fd0c7e8f3a3169f537c6d1362f9b15f5c4d4cc3e48f3b41a66208a" },
                { "af", "4ca1f188f5ea7bdd99b046b59ae6ebca99f16510f758ab4a2d553c56460e939163f461cff30ba068ecbac7ae6bff015d01067fa338fc079a03d887f381f69592" },
                { "an", "a754de572baedabd8df01f1c15d5a8895794c5db1c1b8358a094a002327ccee01af61a133f1575e7cd7eeff47794277ed7eefae1cc498228e0f24eccf674515a" },
                { "ar", "d910ee3aec18425ae53f397caf9c4ac0f4ffd55db47e88d72ec4ba94255712688d27bb80b1abf572d54848682d2a637622f9f68b807a99490df9b3cee2d5a0ff" },
                { "ast", "f94c491e500f5533d5fd2253bda7b8d140309ccf5a1bc756cdb6cf5ff3a25396cfee5c61498e6e9ddd4a262af662421f0e21d88e5295f2e5cb3e546369732327" },
                { "az", "5abca609fdefdcfdc8f1e9905b07a012b5e119ea6bd2570e756801e7ea517b0cdae1c984cd8261786af66812fb5ababf96a7ec0af46f6ece3774f64488ae2fdb" },
                { "be", "cc64ad69ab1ffe0609b37a105bd2446e342f760aea1ca74ca8eb4f5edb47fe8c633e4c28ca018e53696d8483a13247c7424c2817fbb458bd3fc63c79e012b5ec" },
                { "bg", "97a3dfeebe55417ffa149a5695989b0f8bcd1aa581cd38b1367cf10d9f76124b0f0d4807c895ec7b93d7e1c1454f469aa6a9983b68ca610468b255775a9575e2" },
                { "bn", "518c26873d60f936190d0016729918afc9a9a0d45f62cef797ec9d3a94a7952fe39c29a348d1252be626eb49102ffc078962473e30ae1b9643230528e3c5984e" },
                { "br", "6a14c169992a4b987f35ec0b6b799e78e5cf185d046a8f3e410af978ecdb2d8179b04342b4ebf5601f18721ecdc595b8067786ef3add0978502252e3c8cf3f8a" },
                { "bs", "eae266954e0defabf76c16d12bfba0025baa75cfa5d1ea8deecd38c62436d033b80ab8deaeaef6db798d10f17875467a942eac34e1d6a7081584475404c93b8c" },
                { "ca", "f7d70aa435feb570d49da07b52355ef23f0ed73e5cf2a215f945934828002994b6daa655fe64fab2fded6eca1b9117a814255aa91c55fe810fa0c913192fc611" },
                { "cak", "07a7a1774bf4f73f762a6ab69a0fd00914849f857c92253d3558bd7c37996e08e1f638200e12b92274c125ce25b650e2b6c2f661b680052d486cc485a37be9fd" },
                { "cs", "e51180b98e15c8c2e169430e93beba2b404086da1ddf739232330592da6e5594b1bc75646c2d82e4a10e3707d43729128ae43bf4b4e857a586a3f01be8baca6e" },
                { "cy", "57dd92f0ce759d086b7027989005566e27819bb5bb81b6a3e361150dc92e91aebdaf36a2ecba0265ae1a5766cd7ea3c9e971ecefceebcb4ff0353c1c111a35cb" },
                { "da", "3a92f16be3caa0df89f5445697b843e77d7b61e5db6449135b3aa65b1034c85d7050100296edc029ade5767c656779a1904e72eb18dbbb7e273abe1fd5f005a6" },
                { "de", "307eeb916457e091475a9a37abd1daa434b005cb3aa3cff5f553a4f24cc0a421f4c50598de7906f8f06ab0bfce218673fddf9043de52a5b200d49570d43db807" },
                { "dsb", "942306f6d564d6701e214941099c6ba9080999a7d313a713c442176503ee68432a46648285bda0125d7f94856198798a630d4469ec3f6af87db45aefcec405e3" },
                { "el", "f0d00f71a05271ce81cb0fca65c32f4b2f9994f59d8d4d71a3e86dcf846a826dd790b64641df16eecd32e3620e1261972523d30d32d8ce32d90cb5d1dfde8018" },
                { "en-CA", "02846897d5e87e0acdd4bfb3f7404af933c64b2fb12703c4457684f2c4693bffc92086871ce69db56d5fe15b6abb8a7c616c309172eccf1fe9535532e1dbd65c" },
                { "en-GB", "9ba17adfb9190717e2e2d6a8f7e05f5ba8af61c117881d5a2aa9b16fc66c2358cdd1b4c9e4c7b20bebe187302f6684bfe5135d11f62afc5677ec55f03519e1ca" },
                { "en-US", "ba6ee51c6a79b7dbe7abc663e1c325091d03967ed9f385f12a98cbc0d64d4f65975da132d453f9de80e6d8b55873cb6ff6b9941d9d228f443aae8680e5c4c202" },
                { "eo", "c94777857f8d6b154b1728e4de9425c4d5d8748188cf963b3bda1fbf2f673511ceddbae8e9c9da1ee4da80f2190efb43c0422ed9d6ccca47debed0373bfae757" },
                { "es-AR", "7938ceb0eb1992b6df8c4d8e3164b4b12b54e045c6d212ba957e1614abf7be905189d4ddd3021a9ab6cc15b24c6fcc7a5e2807d54e69abc7c528f522d4699b33" },
                { "es-CL", "3728f42c84731ab0626ae84b965d6e94eb8e8577bd35f67ddaaf61e72cc491452f048c90655b9c82ae839acd06feecc8cc4934796395687eaa3f111e89fe0443" },
                { "es-ES", "364aa1a8f0963ca3afb75d5daa69ce174170320d552515c1880d7a7e987952a4debc3ecd9bd9db2a63e95e5941e6c58d01b69cafe3d112e5d386245758642ec5" },
                { "es-MX", "00ee9101d99907f64192d44b633d084920aa7f0f3ca7a66ec2d89971f06cb0b006cc761f012c0a0c6724dfca0ccffd94805105e855cd56e443658241a6906f8c" },
                { "et", "2e456c17996c5dcf2cec976f5eaa01c8535ee3078508fd40b6c1bddb668f228272e61982e9e0605a62177b3d69a2ddd1ab2b9a9b7cc21fe0d0fc6c2188291239" },
                { "eu", "924f4e87575293e25ae3d5321fa2679bd1377955c450196b2dae00be6f3f6c6ff6724948a42916f8c98a669e0fe0bd77eb3cfa2447935370273b7eeaa904fd0b" },
                { "fa", "89a07ee74bcc06180aadb00bf3f05feb04d8df7eff3a6f70d63af01f8d74a63283856b5a0680a4059cf6ea39e9ce77d1d0165c459d59a50faa0b89b87570348b" },
                { "ff", "ea35c6d7935312c2169e3e4f4e89f4507a5782f4c50d49ee4bea532ea469437fdddf7472a51d090bd598a1d5ebdc9148830c56d5c9a230e18e5f41d8dd5a864a" },
                { "fi", "6dee2707532daf8ce7804eb6cc72fd8e7ff9552de6b7b4b626049c6f6f20d00427d950f727cee0923faf8333040ab07af0424e83cebf4c46e945a92f46618df7" },
                { "fr", "731d94ec530aca009f731c8a114687829436336894187450f6a8bcfdc24d64baf9b36aeb047808f103950d5e7fec04818c5fe2c0384575a89f1b29b3f3e00f36" },
                { "fur", "d555273218f49f445976d0e0d5258b30477642045ff8fbcdb76b254b3b83825151dd5dab86e2df1d07f1a75fc6eda1ec31a6a350cac3943d049b5e4d641f0a53" },
                { "fy-NL", "bb59721d9ed8c4fd574c1c4a10faeeaabef37bba1a765cf29f165e0e482bdae5068a93e5714ab756c4dc79fefc1fb321842687cb1771bc667697353e591c4cf2" },
                { "ga-IE", "2c5545646c48004b844b4c2247ef35b2c2301cb95de0a5236b140450cad339c348529d83f50dc3466d577e5329e329f338e7525fb0ea5caf13bb3d4f925078d0" },
                { "gd", "bf63732dd307826bac46e0291d7a49bd6a3803d06d3b8468684637203c9607f51d0941bb3ae301e2a6b05b2f311f475dcda5d641ff2a2362966ca4551481f18a" },
                { "gl", "0d35fb55238979f24441d8e69f90e9617653b78003369f1e3aef0328a59a73a15689f9fcbdfc4c219ee25b7ac4db17ec65d4a57fe25f598b42de81baf6d27c3b" },
                { "gn", "8b24d91acd1c8eee66bc89187cf3acbb0b732a655c10d7c0879b44fe5d94849a5097b04d2abd2c37a74704de0bc24900e37f9a5c73816ace3bcf641736141cf7" },
                { "gu-IN", "012522a39e024ec879e5dddc0d8524e5544950768922942918d6cf3bc17f42308074ee5745d29f6d7abac61a653ef194a7f124364af33ed49f548d917f1ce2f1" },
                { "he", "8dfd6fd86d6f97a461c99e06c7e4c8bd36aadf990d86128747446b657075d668bb5b0e54f567f14ac7e1226fbf7fadbf2685807df56bf6119f95f57839c23093" },
                { "hi-IN", "0347f6df603f123991588a5ff146824a90716e7b5b1fc4c5c7c5b95bff7f9d4c8a8cde1798878acb3b7f2610c6ba12f6fa48442aae938303bb52b2e10d0054fa" },
                { "hr", "4a3bf0df150b26dcc8fe1cf812b68cdb156254067ffdbc812f20542bd36d00a032df646295e857ef2e514d987a3ed404680b34d53d31bda4c95fa08afba2bfbc" },
                { "hsb", "bf627b1b365508891ac8549e660d139acafb4b633d4ba3e80dea2d71ca0f4dd671463363691fa676867f936856c15b2e18af3dcd7fb145a596779195cda4b592" },
                { "hu", "82bab60ff533f675808c0a1d0f6a95788d820fde13ea40ac0851b3f0847b788aa95124f75dcdcdb88b9c9fc06e701eff4a539bf86afae739922e8a4c267e3089" },
                { "hy-AM", "dc443bd1fa7be22bededc8718faf178dc1f904eec5e0c501159ebec8238968739e44aab55cd0a7b902242f1dad06561a68e5148fdd45979f40df245c8975a25a" },
                { "ia", "a1aed9a279b2e5091e1140560287b10138de41240edaff46086a53059f18dc1f87e4f6ff657343e12cb0efdbbc4242b5ad76d89896a8d3100c4ff4cc12106b4a" },
                { "id", "98bb63c8de1fc2d5ff2f33af11778f3221082e1815394bf598ace92661bd7f974c6b7be1a741a00d9c63f96e4c7e8059bb43863c451a4caa9e96aaba469c3cf6" },
                { "is", "a769ddbb527b797da8b028f2e0a6ec7f17d0b1e33f510d35b9d7853ca7927164b762160b46eaf301bee5bebedb257e1768490fd760e0083c3ad4554081b3ed71" },
                { "it", "395943bd8062a485977a00ff870d485919dc4632a31bb50841f4af556333e911dfd62fc9aae981b24111999d298e307d0143b15e4ed693cdce780626fa954ad3" },
                { "ja", "9e991b619c392f8c67af7b9a0a8795faf88c05c091251974886ff61c15e0449bdfcf6209def423fa106f9637ed13e4848b24761af8b137739ab0b123277eedcb" },
                { "ka", "8c73523fad0954a7006b515812fea1c93c0659c6968012668dd0132a465abd38dfd7199871e696c60e821d6f6bca172d9daeeaca219dc767de296b0ae56fe3ff" },
                { "kab", "e4a7a4d5b5cbb5d78fce614f2a70ffed43fa9e3fda9d9256cc45bddcda2544b07d6f4f15609fabfabdc7bf935300bf2a7d6ee9a30757e7f1881a01ef2b21d3cb" },
                { "kk", "711df3aa491aaf4743c310d90197014e83eae35a8949b02c49586601f8cfc53d14483a610d7de6202a9108103424f7866f6ca1eac331c5f4e278f36053d4cb36" },
                { "km", "3938177dc56f08f260cf3e7e74e0b5c28beb3966636a77d286191cc38fae90ff6a9aeb3ac9676091362f0bacf33aa50d563c82f4fd4fe62220658a0e5662a38b" },
                { "kn", "7fd678f14ccf4bcbee26b3ad2894322a1f430fbcfad7117d498d7115ec8d502079b67315eabdcb033934924c38f42470527ed8d711f57858e81f24f952964343" },
                { "ko", "784b1429f4b829089bb5e7b4d6a83f784af5431aba98a1e8b62903c983850919f69f78cca2e02da81550659ce184237f26c2991081982ce8913e7e85bd762329" },
                { "lij", "974c8c47a70a2b9e7d3037f845b7ba64574f42df6420a45eabdf49e18624cb3a18d37628d1c7684963e92c65c32e93e6ec363485dfb00dec6d1ad832184134de" },
                { "lt", "61ae3eb7133ffd8820b9b1334b6d2c7efc388b60d8909b2acfe8386697c337c5d485b471c120f464f77dac740f8d01d0a9fac303363defa5e70bef75d60d2146" },
                { "lv", "dce2a812d2446e3211e7464afdf0a02104a5a8daa18d46d47cf1e656b9096908ad23993db54a8ed492f043c1331dcb59b64c162caa1c756610ca19e958813ae3" },
                { "mk", "2f8805bc0b785899d85e8a2aa33274ecf8b7f5a5dfe708a7bb1f1275ab85fc7f6d8e3b8e2456cdd91bd1e63b0c4ef6ae5e95abb55e4325d263f2806b6a639951" },
                { "mr", "dad54605d00ee3a59c128fe0801a63e1747f97d6f8d7b6b8d3bb726bea7dbc4d6c98c9bb607a2771bd724f848f67e5013a39b93b6f8938f32b8511d131f6316d" },
                { "ms", "45248f76b7af95eec5d3052dbc15334dd5c1953178105a6884e4f2b9f5cc142198c02ad0f18618f6aaa3dccaf7c84896d4bbf328f2b6054b4312cbfa4c5ada45" },
                { "my", "126910c12b3c3a9919468142254eb355dcab1d2abd11980d185e1b29534f167b19011d02306767b4c362b0f07bd3c5fd16e51b8a0b50ab614fb7613fdd80125b" },
                { "nb-NO", "64ade9d6ff873c3e5a5797616e674715f787523d8a02fb61813dfb821f6685728496fbceedee93ef531dbb9df6c9bdfed88ddcf932023b1ade37f2117fbe1741" },
                { "ne-NP", "4f3e8a07e5ee4edc4403dfee9dbeee33064a4bc103d733e0f741950dbbb0209e4668fa0433d93ab7dfa740df72ff6dba5fcc615083ef0f8635fe4c2aa2baad23" },
                { "nl", "2be51b6b0923935fed531c60eb9988e337bd86b95018f36573b09fcbbf3057c7b250066f9d0f94332b441a27de1767a709d25f0728437b4fcb3d727adf625888" },
                { "nn-NO", "ee80ed0f75a5121060410ff8f76e2669217118ed14affd9de373712751d3a772378250526bbb35493867854db75fff83a12c090fa05132525997020a6bef7a84" },
                { "oc", "07395702819419d3a2276147729e1fe40d1c3e90d2c6cd0fc11ccaa8eca8295be4994056b34e2b03cbce5dcf451586ae325007975a7d7d2767a0ff3d6874da1d" },
                { "pa-IN", "241ce4ecf64f26469f55babdfbd873e48004a4c50efde06e8d86625fbf6706723e66c1ac1f7b07f434f3e5cfae3d31509e988e7d6ddacea5d0595ef954fb515f" },
                { "pl", "3b9003249db16fe4fe20f1fb45fb3d1badfbf8b053e03261a0af9f50ce425b67a4ffa45e2b8b1bdb612cb2f1c21012fec32cfb3dc9f2515865b451531021c5e1" },
                { "pt-BR", "24a0a5b4a63ef25a58f45e693a0d8f7ca679b14ce9bd15e21d47c8720a92778f4d2722dd7979b2ef7339734c9a785996bd84bc9ea9f021eecb5e4ee29e536ccc" },
                { "pt-PT", "e88a5c0ac356c5da1c13984bdc52cafbe32d33a8498bf0402d191f4a5dcbf8d026b06b9b650adb09be403c324194acbe85d1fd7ab9dc06562d75106f803a699d" },
                { "rm", "09282a55900dce30392417f13fffc9068c2033d6c3574b2422f5f17459c924a5b65658d07ae7cefff66379beeaf2503f8e582a896d0fe136828bc6e5a15f968b" },
                { "ro", "983afa1c66d735043d7e38858eb8754c5fa1aed0020ac43e8a541beee815948e7562d7cf2c0d2e4755da6d458009ed2c9b2f6e2e7f3e7f5043e6deadb0139729" },
                { "ru", "7973879f405f8eb42b39c7046cb85aecdbba0d7b59ee505c819a7e1026be94fffda1bdd39ca8f663777ead10c11218e437496b264800865fdbea48808d91d11a" },
                { "sat", "ecd2bb9465bdf6e8aebcb0ca1cf771c26eca9d0a684e1e18f7eaeb40c6c5e5badb7ed1babcb7a1cbace7835abcd6f998d070014519c848f28ece91e91f3b1dc4" },
                { "sc", "01d7afaa41be6183c85fb85e8487b034ff08ff76ff099a5dfee26b22cd6c9bc3f357df09d69e4d533d979c9b6355b15bb1982a5b4c6f6db85994603acce4ac96" },
                { "sco", "e9bfe9091a65451026841c75437ffbeff7507d607cc2be2aa4a3b31dc9f3c335256f1e1b8a7ee7adf599981295d1d70f814983ee85bad95db7b73b4464695b44" },
                { "si", "17b4df2c84399a2a5ea52a6643fe561c4489d951fe1917a5af0e7f2f826caca03d6788bd27aaa6e0503f645abdddfc04e80a3646c6aafdb1408a5db06fe7a93a" },
                { "sk", "e6898ba90fa7a6ff30465e523a6680c3fe0b998800f8923be25a08b1761dae8a40ea3a1e4873c79462c336e957238f747f520fc6578006358a42c4b755952355" },
                { "sl", "ce7d0cda0fc1e534c5e832f3279c2be393001a9220a39a7ba51f573674ed26cafc60b65f9281f20d30e86ac79fd6d6992ebfdfd8b44d26ad2f53be8aed7f3b60" },
                { "son", "7b0cef0b46391ba37aeef64f46288724864b016e65bcc39f9ab170b1c243ff7eeccd22bfe2b544bd88d482aeb094f7bd2328fbe85a66c514151c7c3b787c9563" },
                { "sq", "eb2cfc830a78b9b102a062b3dbaf49aa8f348971ebf48816a24072ab053adeb449fcf48e36b4b1f4c1c3e7d5646331ceb56d5a634ebfacb28d43ac37cbaf46cb" },
                { "sr", "018b88ece90fe26ff6b0dde254c7c78df51ce45c4e3c1b6587336bec622e3ca3cb330d6568d26b7d786b0e600ed2350ec1909e02a09f765b6b0bbeb5393d83b9" },
                { "sv-SE", "b6c6bfb1bd62ecca02484387d5f521c35ca604800f3db937fe3c409037545989c4927d9ec013808f542dd5d153797e6f4035d35a85372c88165a2e088178e77b" },
                { "szl", "b6c95c4461526bd10ec4109fe9725d7bd64188cea8d2553776cf06bfec4d181b4dd0353b9a48bb4c004fd5072f816dfe3ade209d3defec27befa72f24e6e8fd7" },
                { "ta", "2e9cf41f089a3ae4865bb743e19d588594b791ed4fc875a55aae8534f6a32c38a707a68743e56de80fb3ddc1acfa7aaa9a6dd0d419aa206d7fb9cd173ba78bc9" },
                { "te", "2a20b35f50f551bf7b4b5fcecf44075ad67ff3a4c120bde823c4a3243da5ad6991b50bec1929ce3ebe82f4131b9b6203baa39b248b8af356cee268a12b33bb6f" },
                { "tg", "a68cedfcd81df29b47c946ab9567cc6f6fe7e3d4c452dc64979af7a393eb4095398a15b9385609fc2faf58f2b05ddc696eb2ce17cdf2fb5ac6f986d66adb91ef" },
                { "th", "54198166b9c68b3126f60df4879fa2f8f0b6337d15208edfb93649a7d811577b21bf243553642bab27aae19ec010146429911a58d636f07636b9e6fbc79e0e9a" },
                { "tl", "fca46bd97d0dc4f78140e08352644db8bc2fedd83b3b0526182eb4df91a1f7395cd0d2814e63d8f0e933130808d6ceacbbb864bd87c1c5e8370f2061908a0ae5" },
                { "tr", "5068769cace55844a81adb4721883c9be92c3018a8c7382d984cf0a71d1d2c4a3f95f566a3dbf6b440937476668e1d8a3d584f654ad1521a4d7672b50276fd4b" },
                { "trs", "4927a9d191fd953bd48b3cdbb598cb302c24059f3f799cd36e0364ec94c7edf39ba8cfb8cd636d2ab10ebcff719eae8df0eecfc42d6568c15fbe21a569d280ec" },
                { "uk", "a70e6241270718e675afd19d2aaef18f2792c3756a0a6b6dca312c09e530ed9d794e6b084153f22792a400889f83a9bc71ed9eec75d70c0cf65a1aa089e9f5d0" },
                { "ur", "305424e1b4330fec0f15ae14cbc6e5edf0a96a6eba17c28c19edbd8ce22256a1441ed6448e39a6faec6ad270ad44468324585b41de703b3559140e3e9612d140" },
                { "uz", "44e5fab2926ec32504b7ee8ae6752da2182a4684a403a038e940732c51bad92c02cc070e8690c1c452a73ec3b11f6b61341e856272762dfe3020e1207b7dc3f4" },
                { "vi", "5166d75a4ca8f5e84016b49525a43d05ddeda90edec8beb0c531adc3fdffd5b8399c8bcb843c9b45d215fcbd8453b75922913aa77a315681f7adc74616ea6893" },
                { "xh", "ed3e1af02de8b1c3de7c038e5ab0e5f60da0a738930255a87aaa021800b9782fd804212b338916b32f2c2634ea8ced58ec40e01fce774d2c948ee2a217de3ed3" },
                { "zh-CN", "515586419d407be02c89cb63d59a353cee0b70c2d2a1a672a9a3b6f705035cbac28a3c4a8756e1966122e389826df1f197aec18044923e7f039dcb0be2e3dc0c" },
                { "zh-TW", "8e9d49ede3e0466cc0fb01278498451afdd0b1963bb826feb1529a2a691ffe5a5863395e0be90d58fc993b4b7202f7096ae95297d8c9f703a6892d6a0c46054e" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
