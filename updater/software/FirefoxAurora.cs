﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        private const string currentVersion = "108.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/108.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "053e39b74523bfa80fbb9858e84e54cbef203de3edf50b85679af8e39094b5ccdbfc955d1c13e23af03eccd1b88f55ae94c2a823beea6db8a433ea2915b00388" },
                { "af", "9580d53c6a9e55303799dbf056546f3b4b5585ff3207ab3a652dff0e8b8f6fc34fd159d237db7706240cc10f1dd10fcbd3e096e98c01f5a18adb3e1b3230eff1" },
                { "an", "1f536101f140771c691f105f41a3cff21ba83369ee070df200f6bfef3918da500c1242ad9568e0ee74179fc2838527e353ca220399597ea5901bb66f5bcf8244" },
                { "ar", "8321ba673dec6ae37864f4e025a91bd1e1f4c43acdd5a2727c8566688b5b8bac8635f68beeb241aa89114944069c9f8bd83e406db6dbd88045b524c8752d6fc1" },
                { "ast", "e03d2076c3aa8e4a4707d4cc908c04f440d29a92dbfb6598d16f5cf47128155a14f6fc36c6134de4cf8170ed76f5e693ad0f5fc6559bd2585914e69155791a5f" },
                { "az", "7fa1566243a74aca7f35cd86ae76400dfe635635181e86a42df44865bf96c8b10bfb26d631cdee388fff30944e82ae6b8bfefce64717882e0072f776877c51de" },
                { "be", "84d18375660cbd513c96f19e8952ade378376e3b2a7fe6bad529524017e8f67782522ca7c754b6e7a804af1b7ed8b557a9ce44c9ac03789036e50b9e48ae7455" },
                { "bg", "92a67aec15481d7c1dd90255ddcc9011a69b8717c547b156b0cc2e6da6f3efc53b88a638d8a777a157b291a2a175c9b97dd69869be5a4b775e9431af858f5d55" },
                { "bn", "2998dbce5327f1e4beb982377b6e8816ef864360d169998d2eb3507a8fa39e4d9c39fc7aeda1672e8c747bbbe7dbb22339077028e491a202dcdbd194f3344874" },
                { "br", "1ed9a00762d8aec35b657c54302329247bca27bdd8a57fbc2d3e6985456ec03f7b68f428bf2efd4bc1d9e2c03012e223ec1738bc226c42e72f072fd838610670" },
                { "bs", "d97f5bbd8ee91b642fb5c17074021b582aec33a8273c6c81ed669352f2ef60d6db745a8e9263f5cb4fe9e4d88c738142e6d4d340eacf7987d71d7002dc25bca3" },
                { "ca", "18e0b7384527859b61587276b9b1aa498be48ae3b48265753cc8000ce8d091c42f3d6a369ccac2003a0014f1dbfca86a3c1a944bf5b04a8b9352518fcc56f9f2" },
                { "cak", "8ed5054b10741849c6f0b2effa4924c82c2429872602811d2d683705c4b894357d84db862c9811b20b7848aaebe0c2a4bcb1f35ee37bd5865265f61523fa55b4" },
                { "cs", "19473aa7d0e6a1ce4c19d250878719fa3673a8d03bf928e20cd16b0939479d1e7c746f0d4b10fc18508606d20f7fbe4e448113415796ad335492e27984c72300" },
                { "cy", "fdd2cdfa451dfaaeacd29a0ff0c7fd9e0575199208d40aa06838f22f8392b889fc20303d798fad06faf67a7a0e27eada7c05b152d479b477f69f1a5895cc9d68" },
                { "da", "666928060fa77bab920470363e1ab7c897138851bb46e5ef43cf764b04ebdf40753f24c0ce6f0f60fc6c71f718ca2a98089e501f090bf6c494d59b638f75fc99" },
                { "de", "44ddf6fa3c557b7054f4e5af819574815604649d58ba7fbc25bf24b9cc5f2d4d546cbd256071131ab27148e17b8ad0381f5d73e098df61dd9946ece7a6a6c309" },
                { "dsb", "e3a1ee1a0eb5c9f7f3f4285013ded9e05e512ad0d9a269af33d2da8252826b39418dbe8575b008160a3e50bb2446afa0dd612896eb0685f9a7f27bdb9ae2ed41" },
                { "el", "2bf73b7a3e2a72dcdcf9dd72a61b7241b7c03b72f91075925afac33a80208b8c482e7ab5e7e95ab700667ecfccbb98809de4bf676a815b268ea07b9fbd7fda3b" },
                { "en-CA", "ae89d22a2da0bcb192d3494ee15fc418e12df1804aca492bfc12727034c7f9996b7d654fdc43363ae12437f138314e04866b3986e8c6a9212537a8e4ec6948bd" },
                { "en-GB", "ab050ca45879c7c9e878cfd428062847fff5a436b72dd5a4d1b62c411cd0f7d430a29a3b7208f931b4a73dd9038e2bea17d4acccae09cb52d130031e04e3c945" },
                { "en-US", "30bdc9a28eb9c029b26a51524af3aff934b6bddf126493618ac33e0c8efd7ff7b25208f36b5b19221a4452a0b4636787a3d4b008e6125ac24366c083f388bb8e" },
                { "eo", "78d85349e682c145d5a0f806b698e4b0d4bae2c37873359c3e77cb1eca51d13c6234a2cd81564abb9ec99c6eb736c99e16c7932ddb65b8d787c8daaf217bf939" },
                { "es-AR", "9b13a9969644a61b298d523e987c2b0695925e7a1b2839c172757df7525a89b3e2c59a02b054b73a8d5e259bec52a480b1c13120de493603b22d21cca7f4e792" },
                { "es-CL", "2e46499d5fc2d538b9d9ecfe4e5377695d3fb44b2e66df1746ffae69de7c594fcb4a5a92e9144de8395919d03af2dea6cdd269850d57834780dace9e7a66af29" },
                { "es-ES", "ae187065844b7ae23c860255197c71cd9576378eb04978fa4abc045e251c18d29f0c7dfa6add894fe941a1dfd9f2540a47eaa028f2d952a20a77a99cf6a2e2e8" },
                { "es-MX", "37b2f783d2de2d72981ea7cd4f0e19c328b0598966f78dbe7e40bc21900bcf991ef67d26a56cff795d4ad4cdece674b45135f0e37c3b9cc30b20db20d0f691ec" },
                { "et", "4436152dd39a1a790e677bafb18e625f027be5a74cc470f00f3056250b7ba132bc0881229265b89ed65e511a9574da390757692139fb6b4664b0d1fd9d65138c" },
                { "eu", "d291da637add13101bd4a63ffb2d32b23a90c9a1c488adcbafe863e7ece705ecd00c76a0531b7b2f7dcb601db33648c0ba687418b5127a7a49541449bc070b47" },
                { "fa", "eb5085aca54c52f8b3160ceb8093a65339133e0b755cd9d206867a1939e00e1dd10219045c87ebb8e70b2c8119c37850f257a2528d2fbe0261007226a4ba1339" },
                { "ff", "3fb845e458958f8cfe81499c342cef29f99275db7b9c8e2c80859229e35594bad349536ce8d8e11b4a5912c6014f80acb7fed0c7ea6e88fbb6d0ca48378f86f5" },
                { "fi", "c25d3032fc7d2a820c475a0d96671685c0509c0a0f1f260d8a2d293f15d33561995922da114ea431a8ac506663660a2bd2160b536311414fb43ef359c1aa584a" },
                { "fr", "b5797aec06d98dc10631eba037c48ad3251a6ac6f3402a7a3be1aa42e1308fbd5cc9edc780e20f96f74ddf7b56186d35f48e15e2e15e20b63acd6287b03399c2" },
                { "fy-NL", "85912d8cdf64003c1a94155b6dc6f9269de11a45350c2ef70a73f71b004bf96a4c0c34acaa7b7f2da690411211ae0b080a35f8ee520744623c728c5b8338bf00" },
                { "ga-IE", "b83bd62a6e1e42e0124a6d1d810cf66f8e3814b91b99e9194a0a11a07c52671069635c456553190efcddcb93602c2d9a8f60bb97bc57ea6ea69d3fa9d76b8eeb" },
                { "gd", "bbff94dd2191b0edbbcd78aa2efee5d5a636671ecd34b86b0dc64202a27f45fe0c897508a17b4a0aeb4612c0483bd9f62eb16858463bdbd95db8e5c80b5ea00d" },
                { "gl", "f363471d9c3e619e72f175512b637857dfe0e6efbac505bb6240efea909ea4d08c96670f79956a10a61f9298a8522eda21a56a86ae34ebd20c3bfadf49d00c85" },
                { "gn", "da32b6a48ef15c2b932f73b8078ce19c2f624780a398719afbed79004e5026c63f7423c0ddb67229e99045be9b9390d1524d890f7be36e406ddb6dc7c6910fa2" },
                { "gu-IN", "348f8dca7359fcdfde9f31389ab3a8508500020d2837180fe71f36687d693192a5f095225c2ab7c333b9a75a1987616e14715629105bdf5211e8a52fa4bda454" },
                { "he", "a2e555d7fcef080d9a2bd3f97222d460a3f6800a54f583325047a29fd95a72217fcc779cfd207a6a50ab9577c5325c152779fdc73e2c6e65e4a27ef6cdd2473b" },
                { "hi-IN", "1c625feaaaf6629d940e5edb50378bca83bdc8214758a362985a81abc38cbe9f385313b0da3bb2fc85580420dd26b97caf0a5f63657b08f5805808e7b949cce8" },
                { "hr", "38d1ece2842af33ce78d0ecabf7219624b50b88ee3f860e53b0a0c3d1b2f1579551fbd7ce072963edd1746a000344c8f3adce1b30c45b25f01675c999f76bb88" },
                { "hsb", "fee46affe5d4fff8f9511ae0605108be927f9112c1af9f5d6b30876f1229461c40d1f816d7e94a07c4dc6fe5a12cc8e4d77cebbe9cdaab9ca840b5be7daeed1d" },
                { "hu", "46bcc8d3ca4a6858b0a2ac2f26c10d3a5ebe755b41b36c4d2db32a9cdff37753299b0344103eddb99dc19dc65f1751578ed917af2016ef17703194b02d51305b" },
                { "hy-AM", "edfeae578823b02af4d87e6dfda2141c1321009b589545f61470ae31c1bf0c7b75608d3691bf658cf71851dbe2942bd5af217a750ea4cd761009f8f2ed1e73a4" },
                { "ia", "1e14ac946877d5c99654e8009176d9f8a0ae0ceea1c5d55a161d4f73b3517a07038873ed3014e179fda01e07f10c3f92c7a559822e3581b14bd08306590f2d93" },
                { "id", "8056799d06a4edfc7b3b28344e213458690e39db9b619847792d764ceb7b4455b6eb03b249fd7ca23b37c84c703f4792383235568c2f1ee5f457d795422975e8" },
                { "is", "90de2ca966b881d17e987f7a4f66e4ed3f03c8c5f8400e289c07fac4b50cfbdd54b5bfafbd7634db6fa426e5713e252aa5e20554e06907bbeff802991205f956" },
                { "it", "61f4cee36d43402f9a14f673b18f858f1182c5c933da3ad8ab32435fc1dbce8865a8103e720e0e889173b6cf8b423de6cfe8401c0621820107f7f838fe74524f" },
                { "ja", "8c3ad60037176ee9cc5b1bc63855f42b318a12be6ae9e382009664644a92814476578fd89d3e3031af194eee4df753c437084164ccbbf8ce1ba155f53489bdc6" },
                { "ka", "062a0f89a27b13dab23a6d2cb78ed390fd2ce72b1b5e04e8cb67432e592fb1f186677b33eadd38937888e3b0b21e6fd0d798de32cf833b2bfeac07c54ea2ac91" },
                { "kab", "c92ca543d989d0b1bb7c300061dea60c4faab5ada1f52d59761321798805a46258312aa593d59c9350fd2efcdd65865799ee2213a63cd5c2129d9591da50074e" },
                { "kk", "564d85c983e67ea7cbab3d4eb31f5bd5b5ff2327a19f895b2063bd017e2923224c1ef891a140c179a43bb73656a6874ffb17cc25376ecc08156110a53e9b9ff8" },
                { "km", "3b9470949d07c7aaa4a0bd88248727be90eddb9a5e5f18781d4415a36ec6d9345c9e8f79f11294ded2a5d8f9d1d8b442cba89006dcf33cf80200f18cdfb54e1a" },
                { "kn", "bdd354fadaa96f0b04fede3c94f92e3549a03d84fde9f7caf14db41f8d12fd93a0d73055e5bcb5372fa4d505ba54a62d408f0a348ee89d0bd944263eab75c058" },
                { "ko", "119a39499038041f5f13b48597c2792467d1d57baec90d51de11fc931d66ec6d35cf14e570a85e7897ca30e91f2ec95894a973d8f269c10a6c19241e90d35f4a" },
                { "lij", "afe1442d37e33c85614c950b5dc69c6f9801a4db9eba37d6199a0df6af2db06fb053a090fb9a677ef0e734e1358865f3f6efb1b58aa83fe34bf8056ebd83a7a3" },
                { "lt", "6d234ebbf7e51f47b74298239ea178f69d2726a04b63b32e70bc55c0652e0d2f85ded2229d0210ec8392be7df5b02e319ee709a54b3237f8bee9697e880604b3" },
                { "lv", "f0d768e2e4d9afc33073f7c93489ebc74806929c006ef6c81f97584649e8422be168c01620869883e701059ffae72ef3642e9903e60100bf2acf4d5c5ee93812" },
                { "mk", "563f7fb76d66a8d570b3dd4f5f98aa94749524e665a5bf6c50b6612f6ed48b893301fb8c1d4249b8b39b8a964abba75966bf1d7d315941e10d9c14c0d9876b0a" },
                { "mr", "5d9eff109bd4e5b27754e14ff7202926d656fccabeaefbb132d0ea675d3affcc77273164dcf39fff57ab200b2d356873049bc37155877b8184e35198cd46dde4" },
                { "ms", "4faa51f7ee60b392b234f94f03757e96883680e101c460b93c4deb4afc6f55486d3d45793644b0be2fafc7635bf8e1f74be4db9193a967f933ec8cc82c48b8a3" },
                { "my", "da055cc1c70fedbbf561c3d0970698f3a2147350b65adb17bdc1937490fbd8bcfe2883b2136392961dc0f28c9c70d6da425a870d62f3ad5b11c3b076ddf76b13" },
                { "nb-NO", "f943ccbaf51bacdac7c5913c46cd90067c3b1e512b76eee660b0dae07c72e35d7dcdbd32554ca1e8425bd9d222b8fb7aed46da1db3554e0bf9c70de9807d51d1" },
                { "ne-NP", "8600c843a705ea338607c302dcafbb15882f8187be639e65fe5bafbf63d538b5b5108af561e214e3b54fc39d360bbd1acf75a629bb1d3089006147065073da43" },
                { "nl", "8e652f91bc89468c7808a524fe160da9b1a8a19cee2ea26114bf1c0111fce5a69272817d2bb01289472e3f1ff55a83f43f008254d6b37210eb06493954e67811" },
                { "nn-NO", "3acc112322d9d2647e8be80b057504b85de946919921dc153263098aae0b81dbb3819d2dc04be05a6e2326d22dc187b7f05c89c0f2a45be3ceb62b01c68fa6fe" },
                { "oc", "07b1ccb2b46d6cb2d9b2bb2d87420318555f13d3c17c651982b9110c3a986ca381f5bb9f7564cec8bc171089a93eb98333269f8a7bf9ba3e0a0d2f18604ce27b" },
                { "pa-IN", "d9530a888f20f12065a2bb08af8ab6b3162e268327cc13c6bf1e245dd96a321855c9ae44610d3fc8ab89c81247194bc6dc55c1e2b50e8c75006cbb37df680c30" },
                { "pl", "2e0ecc202b4aefa5b497cac6e1016d8805a82e0b93c1e5d868ea58a45fcfe6568fd8a9349349a187769598235d1b0b953c893660679baa7c95f11b3157548f80" },
                { "pt-BR", "051d46f50f0e1f508c13a7fd095ac7177794bc6fa21e3da6a0e583e07f2bf6052871dfc696ca59d1dc0a5334cc77fd86098d7d44e488ce2434651a88e544ef01" },
                { "pt-PT", "0676fc714d82fc79046d5bdbc7c3bca82513078199438f34ee2525e798fe76b03c11802f6e10fdd171df987b01feeeeb746fde05631d1fc3ca700f83b3fa8cdb" },
                { "rm", "9c0e95c52dedc2461b8b388a2c13e2303df64ceb5059f04977e5d81bf8bf5471c18de0b2b5e5e82ba9ec17c0dfbeea5174c0324f4cb9bfb79e0c0c9a4107a4a1" },
                { "ro", "98faf855c3a59920aa6a8f989dd7172313b3c7fb8877aaa4b17e1a938ddc2c53eb38e1bfbd4922a9d191b21d9a807fcc3afd27b8aec5a23fb99a4c0efbbf0ba1" },
                { "ru", "fcbbf47cd4929058cb1d34c38d03e8e436d49b2276efef575b92db3ce62f4fb0b95fc0f53a1f643dacc2fb0d93be40758c3e7f7926ef0335ec0ccc1b51876807" },
                { "sco", "56e8fb316615df9eaf331392d111abef03d21d2bb603625f92a89768bef5c2c2ce474b97f2996f7f1fdc5603bc4cdc0f378772fc8071d7995a08da875eea386c" },
                { "si", "6c18cf2889fcf06dfbea7ea127ae00c50ea54af7e2ccb8027760d3ff0eb070076cb194ff361a247c48ac5d7ac1179f3614004511d07588521479ed601b1c5e1e" },
                { "sk", "3d6277d242baa30747df98099678b0207032de70ec3b13af02c6af460af9dd3f46f77473fb5ce4de6dd4fb27cd17bb4044e71803371ab9af7257eafe11b5a725" },
                { "sl", "fddc6c3915d0a9b6fca2417cf239ec71447fd7673d9b9c746a49ca8bd5285236c205c2deec892d0b6bb574814ee324b1443c8e0d62cee0bdce2f47143a158a01" },
                { "son", "84ead15a5814e99ba84a92e497a401a5302065c796048f6cc865baadf8b3c68b4a834318e65bf11d0409bcad261f8282cf72661cb82d091c40ad24dbe32df244" },
                { "sq", "949f2abb59931d2dd56067fd5f8f908623e961fb3c6a22f239623e9b84b8d59a6c28a505f13718fbcd01fac62c78d57be08e4ae5cf93a64205929d8882d684a9" },
                { "sr", "ede6c8edc2644f929280f49a24604281c4cdc7e3eba9696f063bdb04fae5adab3c43a57259efc1eb1e8f703353e29ce0bd9a7b01b46ed7315156645c9918f5be" },
                { "sv-SE", "ed8c4aa78b3eaad4e63f32b8fb69f63795ad2ae887ee435c29baf5543a906d4a7f69be9b58f21e9398c6dd96ad23d4275718d8f353fa166a87c57474cc08e0c5" },
                { "szl", "b7a1b8cd75a770f31bc7cd709f5c5d9fcfd580bf30a5a38003db3bdc7f7bff7d02d957f00b9d9a693751b9f1514937269280c746d1ee157ba5884b11613f289a" },
                { "ta", "9a45f3473767137ffc540edd3d96fc824c74fbeb4fb7c50c8d4643ad10ab82b0c7f66f7a8d20dba7d647512782ee26259e3e5d7e054c06e2b51b566fb87515f8" },
                { "te", "f10361738bc24aad81733151cef300eeb5307e2951406d1f6164a61725b2fb9505018058f8a95ab4a9b43e20e98c4c57d4ad01e3b2ca21c4157bc0c6b8af058b" },
                { "th", "999ef18c7e38f06bd1959daa53e1ec7e446f8ec7ee8e1c73f2a4251ea1f39664f59a7236ef301ad282667aa725581681b022930a0b70a822c5b662c377b3effc" },
                { "tl", "eb7e693159f32a1a371884bf5ccae36ec2a458f370912fc8ad1f7da9ac3a53bfea206e4018c6a7b8245e72b7d090b700cd7cc5329a8467d302b99832316dd3f1" },
                { "tr", "deec0667e9ff8007624483cba8f25f2c18a85bc2c9bdb980c9ed970e557edd937138adb5588113c3ba9c12e3822c56d2810751cc2fcb2d4a0fbfae3da3a196fb" },
                { "trs", "36fabe4000959629ad7469dd4d46b5a4f9056a97f8fa2f7664b3f803e553f7956cddd957a79c410f09151c973c40fb32ddc74097e08f8aab0548dd76e3aa5edd" },
                { "uk", "dde6dd4a1b4a9c96418621eddd0fd60e2993ca143d1788d76ad13b9ce497f2112f521f95ec8a1de651732bd83737313ed1121580cdda36df43cbfef091f5fc5a" },
                { "ur", "b9dad36fb1a220a32f9ec9e468fc1113d20f9078b0d0e7a80a0d1d068db5213aad90763fc53f2fc437cd48311aefbe135183d241d5d5c45206cae51437311be0" },
                { "uz", "ccb8e89dcbf96816a39e3619c60308ae9b2d01fce10c30bcbd069856653b96bfbb3826e55cb20e69f4b10b0d224e84ee4236f2440eb57a9e8b579d02934bd7ea" },
                { "vi", "e4ddeefeb3e11a7e14ee7fa10d8dc3bbe7f4fb3c5f6f844fa750f764c445d06e7b7c6a152d6992fb0612bb68e46338dda163b8e92b7463210edfd2a3cc05a9b1" },
                { "xh", "137b920c9e8f0a11545da42b6a740bc9609f596e45ccc0d1f4c2b0f603da3d6ea4bf3fb7d8ed22dc6aec6b7b8c432d0f6304465051033c84668526b6c7a8fe4d" },
                { "zh-CN", "fc471c0d6b6bb07945627e236882038eed5a965f98835cb7adbd1caef8a40a6a4b4ddc62b187dda84e2e70d7473c1b0fdca5ce9fed62db5e499fd3e8f8c4c967" },
                { "zh-TW", "9b2e0c951681a23b589a8ad1b83a6bb8c1b900cd55b1793fcd6968b15c944f738331182c45453f204d14df191ebd6ca82bcdcd949208e996528cf621f3fd2df0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/108.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "53c9a26a4f1a5fe76333fc42a74d804890a3331572fcf92484ec4ca16f9a5fe0f3057170ee166da5f01ab8a0db6aec792ad38603e302956ef4a63370c751e6b1" },
                { "af", "20fdec1cc3f6c5bbe11723355ca622e9092cd79e80c1abdbad98276425dac60b6095932caddce119e6cc057f37925e0e785a1a70c05a860a16188f04941884c7" },
                { "an", "e68bf97edfd322d535d33ac7777012e7ceef5c6019d1c1b345db68760dba28973f9f024b0a5d903846be64966f02062a8ffd5ff642f3ef8d8494c8645c1ce7bd" },
                { "ar", "1bfbe8841cc7ad03aa8886f54ac1c55544e2ee9249d8622510b2e81d7eb94f032153cad5e81ff83f2d9e1ff0c174b6aaecac74a42be5b1f593d14d2dff3a1efd" },
                { "ast", "194e8453b54452d244191802a2ec2ba83d7f995ff7c159130bd10239d06ff7c051a85caa5dd01b371dbf0e98b831870c977469517bcae6ba9a6302ae8923b2b8" },
                { "az", "0ace3b5f9046f2deb9ac08e4f06c08c0466f782561c2fcc67d1463887b48afe2f9ae8da63ec4fadd61dfd441b03e842d4ebf28f0d97b1fbe3de41e83d4e8ba99" },
                { "be", "b9c0181bbea14aab5df2ee32a6ee0851d174d7f2159f91c79412460ac92c9242a2fafb02c7f11b04e1c7722518447e56d23c8ed26fa8fd8e68122a264d9875cd" },
                { "bg", "5badfac33e153258f1e41d54910f01ebe667cd9ddc538b4651a6745238a5bceb3ee9d221c1bb5f6416dbbd7d81aa3260a152d4867a17a3dee197e22a0a80acbb" },
                { "bn", "86def184a98e5cc040af49ebb3c99ae24d108b69d285745324881b5bca7f0bb2b612f3514319e086bce9d9058fa2f11abbe3d4abe093973ac22865fbe6e4a9e9" },
                { "br", "9618c7d8506f762b0a0845d01581c90fa428cbb17df368481148b619530d5809d0267fae154184efca13a88d0731dce916eff78e335edd206bacd619ea38e93c" },
                { "bs", "45e10a5671839dfdf9e895366893e4e86589a63ec6c2fe9c089250c3587023b559b39c74b7f93db919ec1dc8680b9ddacd9a6eedf1f3b9a2f2227c3efd3853ef" },
                { "ca", "1e53bfc6a6f3aeb2b4969d489cf65ca81e9300e4e43d998f023d0b7434ea353f42e1fb09dee3bcaf38047c1df8f744c87f1dd384f0d777f305ee40c9710eef25" },
                { "cak", "82ba845a444751ef9b0b73fd51b42e4184823a90d11003ae909201b9a0291c4cbf6e8bd1cbbb38b6848a25b18dec4f90bf91e8f0af569fca9ca5c5eee0197d38" },
                { "cs", "49cfc4c4000a7800060600ac15742e23b20d35bba2e352e0759a8b81ef1838e015a904f62469044b550b9c09565bebec21e55744af537ab7cd66f8ad8d37e974" },
                { "cy", "ab0672c94175a62afbd817d95eb80397ce41cd14794db5f0e7ff313a1ed4a1bd3031a5812a2d92143cca1d220f7eb9974eb496f78b4443f780730ac37289602d" },
                { "da", "d8580c2878d7fc1acb8165c5fe7ce7b2d6d68b428f152fb65830be930e16460cc62c703b265b3b758b8b71d4cf1401a707a36edde872ee052a2b875a24b7873e" },
                { "de", "a80cfa18ce48b810bb24400729632f88166549034fc877ae1483d30edc2b9f15061cd7fafefaa5a529841e5be17df5ab4d92626c5c3390b15a774997ec10459d" },
                { "dsb", "08401bb229e38fb885410b43226bce56f5555079efbe270269221e119b411a5538df12ddb423865c3c7785fe7d7b51ca505ceb593469a8ad174e3dc03d3c292e" },
                { "el", "f3c10cb0ed596b3a40521e7a97371a37e7f0439b983acc562a490278928e982ab652b6cb4633cc4cf8b7d6bb73614bfdd0655b7c33576e78f7369c3bf3813236" },
                { "en-CA", "bb49e5c026e176391977508cc6e52b16ceb3769db5731798960ebca1f91dc19abd00196e4f82dcb2548e6ea575f375beb0d4ab5445bdc93a7f3ec954c7516e33" },
                { "en-GB", "ad8ffd6986bb7c84cacb889e4d92870dc1081c88286d0b8f259b64f791c6ed17232fc261a71be019812d2ffbe3a37cd49fa7331fb946b2fae0033686c42580f6" },
                { "en-US", "51c960d04af328d3f3104e1c9b696d9366c50dbb3c1f7fc226ad02879c4f62a0b33953a4b620fbd79693d1376865660a5910c8062fe9f4a167cad549b9837cf7" },
                { "eo", "a9284cf81086d2f3664a6b3a066a429ec8da48dad3aa2a04890196776721b5ac4544ef6e98694539d280ddc023c675464f4ed13ac3a22ecb6870df1f5ba5b60d" },
                { "es-AR", "9f2cb06bc8fa4ebcacbb9181612a37274e7cb7fb30e270ed3c5a0b83ecd3d891cdf90df8fb54ad817d3591e7de3cec21808e8a7e59ecebc620179570e87714c6" },
                { "es-CL", "e281e7934d503ed5d0915e575d86f9247b4914cf718d42bb4257e4a404d78dd7dc0370bf4e9777c04fdb655e3aaf99c3f86aff37399fa7d4f715b70217da4065" },
                { "es-ES", "e03ea7ca2f028e70693febf5c28a057b5191c122747bd42eb8c13668467eea5d9947a5d6a3565311971ccdf2693f408aeefaba6116f63526bc5bbffcad7fcef0" },
                { "es-MX", "804d75b903dea9abe2dc157e36fb24016e297f9fc32ffd8ec69d4486d9a46da0f273599e0d9a17fc107f2ed6b8f4e7d151ac53f370526d03c9a6db5d7c775078" },
                { "et", "c7957701149b50eee9e9936e25aa16a0b1bf68701b1e98424fd2554836381a4690158aad8f69907be65085cd2e698a868bb24605339c3edb01e98d9acd4e4c8c" },
                { "eu", "2624e3871feb7a6f29ac6cdf1936092cd43b29395921c5f812380cab0696f4a14c874538c15fad0c8cdbab2f775a8a4af21b074ce33d9fa552106a8ba2b4366b" },
                { "fa", "f00d3073ee3911f8e339e3f0fe85fb9e1e2d5bc4a8c8fd64023e05eb2dc3f815b5c7667f7df3e2d8bee39f25b004abe65ff0ea7cc4fb32e6ce84bff65f5ba2d8" },
                { "ff", "3db9a04181b05ec25dba3cf97b16e241323f7f2c7722f31d0a3fb43070e4d0d3ae35117d4b8c7f65c5c5be6e91e0df39a6b533d5d7ee25eebaaf27fd67029b41" },
                { "fi", "64eda1f9f947e632df07e27e0b78633b01644316357f1f6496af26ee0ee2dfe2fb8276ae6ae8e618b11fcb2d30ed6da8058549109731c05b5cb5f22fbb15a74a" },
                { "fr", "24a0694f98a2f52e6f39e022efcf64b080916de8970bb8a041002de59ac3f4c24181c79f8fca69e06393d96373db6e1128489e47ce441c64ac6ea4e91edaf306" },
                { "fy-NL", "9e61099a3a279df6771b049293faf4f5a9b8626ece6cf661de0d1dda7b0e35338e7a467f1618cb3bd28c897b39492bec663900cd399e8798280ce0ae3eee166c" },
                { "ga-IE", "7f77a5802d45cf5c7fc93e0635bae17d82916cdbc9e100741f4c8a5ada1762f5c0a7c394e8bdfc343cceeeb32f398ccefa7d8902e8b516bf383aa8fcebff1242" },
                { "gd", "df37ac94edbb0b26dc5f13f64154bed9d9d0c14eb6979ca529bbafa86e37ea3898622265b0ff3d7a23f49e24b7e6771db0cb82c29d85c625de15d2b6ec95a707" },
                { "gl", "0602f27d5b46ec729f9e752a4150669e460417ec7c0fe77f8a22bf1fb0a223e3a035dcd2ac5520cb006294feebed8315aba72a498fcce42bdb8bc1a57ec11a81" },
                { "gn", "1b56a31a47d99afea2b6513854c2feb4494ad83084f07aba7c98fbce911e4e20fe586b0ac18a3a9ce6725ec8f08fcf79a21b3c99c22c8bbcccffb5ffde33813d" },
                { "gu-IN", "ed66df7df7920e080af1492aec249209bfc524a47e36d95ae8b1b613ce600784b34d30c5556ad0ad300ae643af8fb9b379eeaac3eef42e6bda39c5ea2ef78dd5" },
                { "he", "d4a97695c394217b68e9f726427849a713b8165b8401c149eb4ec0779d21ddbb7074c92a82bf2c67348f1f026ad1d0e0c56997958c4b3a691f821bf41895e581" },
                { "hi-IN", "1539b0a8bf6cff13366b683d4de5a3fe30745abdf293e6dec92a59624fbdc4843bd5d4826596d6c6b9c97d2579f3e9aeb18d669b3d296ce82e3ac9f40d64d809" },
                { "hr", "26c8b293b28620301313b524cc16cb392f80f22048c169d8f5d6641fe0355f3df276e8acf4061a691135b385d7d46966decb898404bfca64027af2a2b13ab296" },
                { "hsb", "d1b6d562160ba1bd56bc9be42ed40598511449e931a77c2511170df44fcb222c0713c03ff933608e0e4cdfc8c60c6e626d17925b7a3ff959cb7b57bfe33b798d" },
                { "hu", "5d81c0056cc8cd426a6883bbd18da09e6178c75b0e20c90563e77ef1d297825e02033fd562b505a54d63cd973594a4808bfa94c2a4a988ff7aedbb55b21d3bbb" },
                { "hy-AM", "feddd59d931a14c02931a5bce3b9c8786478415b0b1ea5a9fad2bcff58d02be995d261480b1004c2ab101c84e7382650bf737b7b9b8efbc38ed3294fe90f4c48" },
                { "ia", "33b336e09e234131fa4ff2db7dfc1be549b6721dd3157cff176b547966afb6ecb9d8131db8d678ee233b26ab59c5d6fe7383fbf25398674e323d6d461f48545a" },
                { "id", "797fa937ea81d087f48aaed0cf3483560213f27ddc2b1fc8d2f90e1543ccd475d52521c14e6eb64a22739e147cfc3ff57663f8be58e1e1c58b740b1fd706b8d4" },
                { "is", "db8ea08210284019008ee9d4ee621ff8cd2c593c0230a3764b39bebc718b1f3e4c571c83c9630394dfd695518b913f275f36e396ecf5e940bd9cd7a4de279046" },
                { "it", "988d0fe964b11dd4e4cbb01bac65d0d8420386e08b7369bd10fb0226dbec1b3276f600188a9f10cb872601332e56aaea64ca9738bbac506edf5eefa4a21eff4e" },
                { "ja", "4b35c7a263510ba229f92c04929ab7000c2874479ce8c943288e82dddb937e6ec769ba0e6826cdc62e4f3741ac3d7761008a74b825c9836d2e0b380539be87e5" },
                { "ka", "99fb24d295ab4a622c140a5abdf6cb49a443a7d0b32a32a872ce8ba0d43dc2f1e21f73f1442aac501fe181a7625e86611b90f1044864ec5da304dec3507c6907" },
                { "kab", "caea329c68908078c793abf5514a5d9d226a979944764e0d982e6c76df74428b1557df7f0e9fea1e55d987c2dfc74fe39d9ed4ee6e257f935fc39557ceee9882" },
                { "kk", "dc7908d4f68347653efbe35caaabf816404169607f055b2c120d240aac397d25a7f43dd1f3df2c31cab0af42c1b02ce25a1c452a89c57a2e75d786db1eab4bb6" },
                { "km", "e8dae6c9344ccd39e8b32711f5ce6c4ae06b9de2fbdd6291a6a4e85f4d65c719f28be7f3561ec752774f44775a9e9a460cb6e00e8c0fd03176d4c424a2f37351" },
                { "kn", "44e5b4e7aa05be0061e183abb9339ee375083d37a0f759899282b0bfd619e1737348b7d81784c34b7afd4b4e4d2f2c570886819cd7c031e0f13573673784a96b" },
                { "ko", "e26485d5bfb65693b4852b1b44fa3804ecd6209bca910f49637bdae9d4504961c855f2e705068ae875520f199aad81184ad8cc865ce9294e83cc032980fced69" },
                { "lij", "38ddad248f6e0b84812b1eb0c2756f7dc2bef2eef8b7953f6b595267e2743720334808accaec40fae3475c7bbd1c69a465ce7590b32631ad0bb1c4b9cad304f1" },
                { "lt", "da911d5472dc20f51dcdd6c55ee6a9e3d1e055ea849bda4f8b6dd504b1c3252ac46680da3653efab75fcbdf33370366d8359fcb9847c6235cdea95aaef39c295" },
                { "lv", "f71cd53e4728fef151316f62b63688e32d3d15d0a1143bfc6cdf4e974edbd402c1623ecc37a5f5502081109358dae705eca4b0f412c9b3c3973d7dec6605494b" },
                { "mk", "e7d609c01423fde4c3700970005edfcc56138bb7eeec6ce2f37432e08694d276c78fccdab55ca7543260f047e210ce8baea0b6ce817965addb5ee9353eee2137" },
                { "mr", "59f781b3a5b961c33b5c5ad9ea2ff5ff046accc94c1fae0dc6f0e504642c4496fad1c1fe03b26ad77759b47b04381e875254e542992e08f918f0851e18e5f09a" },
                { "ms", "a02df57d2c86c30799d5354441ace47738183d42e3d6d3bdde2d7b33370b4998269e9fd9f9b2280967b6a29af795c4d59e1c592c0358eabbb765750d17888aa6" },
                { "my", "8e2375fc08d22e189b8b446602a53e524c6d266e1c0c253c99e227b76a2bbf539ea64d4155d1e1ba885eb1cf368714485b3c5c1fd03351e34eab1c9de399478b" },
                { "nb-NO", "90caca0374e6f6444bdceadb6cd893740ae9aecf224850151cf396cbf88fde868f7ad1fff5b2937ce4094410397c17458312a6c6ae77d6757b45425552a17276" },
                { "ne-NP", "c17b58834c4a3db9d6a632c52f0790c700f3a7da9c941fc18ff03a4a9066b1818f877cbbf65f684d00dc00b57a2f2970999429f3625a744ee6033f44237ed349" },
                { "nl", "b4c39fc1b3bfe12014a14e7b6e30d4a4e7b9b615e20483292f5c042e517495c01ebb9d8bfe7b1b158f598a1869df72b378601ac6a6e41095099f586953cf1088" },
                { "nn-NO", "a5ed5c2004b10025bbed0334d21a2d1a46c0d8a2a54d15cba784390f6002c903f59085c1945b89c663557f740aeec466a5f8decc141354b77895e74878c77560" },
                { "oc", "54daeff81f569788d692fbe06866fa2e1159eec192e96be87eb12035ee1e161fdd8545269f4bfd296c524c5b312593a905e600a8667a5d9ab54eb6a5a421bad0" },
                { "pa-IN", "5ffe643070cc9434b139b39d3ace12ec8c43d44dc6116eaf36d83e14783750a1c33192846efeec32b16031ea7b9edf28431ad676e72e01c1ae458b73c1a2554e" },
                { "pl", "fb7ff4a15a7d506cb08c0d7ba48e5a783da3180094e07798a27c5d6d3292fa35ee671d0f13415928eb083617674fdbbc99c02baadcd46997cddaa7171c46741e" },
                { "pt-BR", "cebf301d5d756291484fe1475435d16c5f59b3496257dd8e292374ec8ab5a864aa167ba10b843d36092e762a735f73e934ee301689ef1b707caf2a41784f7aa4" },
                { "pt-PT", "fbafaa68232c458b9326b6fba497e7a1155b232e3e54c316c4d6230ae2d5a466f9d9a176ecefbea107a960c6296e763ab71555f1fcd5e6179b6becdb1b2577a7" },
                { "rm", "433bbb1aa96a9f94e576ff45594f120577182e417098c2c13e3b1f3f173345dfd07c04e9ea3aff83eb15059b3b717d4ecc860ef1e341dd18d9ff911097520ac7" },
                { "ro", "cf8ce39171b5b8f333df582ec8494bf06f29a45c8b438f722086ef136e728982de4b5bbbf634fd4a50282949d76c594ad5d2db34baab83b595e93504634e4283" },
                { "ru", "fd8d24762cfa72dbc7660e72d54354984f42563a9a5d50ce2f8e01b6732c8d6e324d48671221e715e4c14585cf5817d9b0ec7bacdc63a7fb9a62cddb95858c25" },
                { "sco", "99d87ec44e07b7141ddf52f94c1e8cde5ac99186a07ac9c64e8817255ed7ab2c8e0637968e55c73b896d8cdc7be810e4a067b2c6d69eeab6991f265f240ccd4e" },
                { "si", "495f4e234ea685871b500d19cc4e943ee7ba0cd332593df33de964a940d56c3e5cf01c54df618fdde4bf16c96e72523ee546a18a743685efd0696906d2a9d2d1" },
                { "sk", "9aa2616be2e007502bd6e5f0f1897e551c9e2bb20f7e026a83fed20381a0c1cbe69b74a9d95a2a96fb21bfd9b958b77287d37242af1c438ee21c2816676aaf50" },
                { "sl", "d169c680b61df948a0f99a70aa9965f30425639aff6eaae7703ac65b49ae35d7049e2e4b22229eca8a2c3d88397da0bfd06c36123819a136aaab6c199d77e8ac" },
                { "son", "e271d7c72107d4e065d679a2ba5c1d0c5490fd37ab92cb7b133472b7a876b482593ddd6785c11d9165d3b8cf7f78a58c7bc1a882a8c5c6efa4fa0d3da907b81a" },
                { "sq", "e1b8eeccc13c18b6ece244d25d2a07c9c3d544578ee857af04fbcf5c67a118bf160617aa65165b45bb55c9dedab176d11ded7fa12dc8d73aba4e304fffebd763" },
                { "sr", "d02b6ad3ced3e8334aa9cc45f6f9c3e2e87dd8487bb9f29960823bea7ba2b11a959fcc1e9f947ffd7bfdc487f6d4619022b213d2a4ca9e410e8923f3d14313b4" },
                { "sv-SE", "b2ea5a748838f941e6f72fd1e537dd51c630f8a7a274194627142c78a7aaf664a839cd01c96c375f12f9825d1fc351b9e27675d8ebe06befe84be2605fd1aa9e" },
                { "szl", "0efbd22c23cca285dc0c5b46e2ba5c88b06d170ac520db8b8575ee95dad163dd2484fac672a12fe312114857a4b23ac2ebac226344f556c257517c4111dfcb93" },
                { "ta", "95cbdfa8a18354115209bbcf39878c4c488be075afb33d06205e4a48a30fb07ee6b84336b61bcc466198b3147cfada4e5dbff22f6c8c0f6e688f73c46f0aaf85" },
                { "te", "693f220502eeb551f08194b2b0b4b94e001c31f44424e58de6c7d65c8dacf9b1249108d66c0cbee15f4bb9d67f97f466d05e19d5afc034028172dcfccdf42d1a" },
                { "th", "49467d35365577b7f18942c44a79c5ba3acc5b6e41e00de57309e01796f1df3f89fd5efdfb1666cf1b99683cc03b0d185b07ed57bcf3247f8940dacca0dfede2" },
                { "tl", "e231d5216da04fa845c51c42a8486a76b690306861c1baf5cd475bb9178cb1876895289ea0be2f81e00f98b799d89b72ae39ef60526dc620da0b649b8746b379" },
                { "tr", "32a3517a8386f63e9534e94f54bd0db08efd59d5ddc0abc2ffe0dacaf4c8321ffeb3321e8c80b80e0d55dd6b9596253efe3b8d6d7413839ca9218bf8c93d4009" },
                { "trs", "176819a52d5265a1ca84868eab83fc4667a5674b161b83ffd3d6a067faabe5f7f0309c919e58c783ef79c813d6c4294a3652cb7027ffaf83f7b2773db73d4f14" },
                { "uk", "8bef296a714e32743ec67c87acacafc328fce30ca5b54e508f3e3da8fbe66596272eaf1e4f09d48bcfa551fc0e9eec93d398d529f19279eb81cab025ad16feea" },
                { "ur", "4423de72acd32cb8ce829139a9964800f7f035ee1805f4eedbfd6c2c8e183b30067282b7e65eccc2e50b4bf9809c3846a285f6f89010f27cd0df3fba8e80cadf" },
                { "uz", "2dd7cb529a43dffac8193ea79a4e1f8ba334da11d5b451ea78af48bbefb7d8938966ac918e620c055db1692af854fe45c21c658506f053deebffbe8bcb0d844a" },
                { "vi", "f183b74ae280cc756f68b9280255909727f0bafca971b66cd765732ad0fb21def7b53d94e4dd777354bd253e26899995db0390b0130d0dee5976dcf47127dda6" },
                { "xh", "d569064b29d742a83d16fceb974ed05abcc1d0dd8ddf495978a526c71e07e8bc6df61e8f8155e83f507cc9b3add6a99d74c623d3d60fda45a6344ea3c21a08c4" },
                { "zh-CN", "48958dc7eb701c025191bc8c11900d15df7c28a2036d3012771e30ef34384caa04897f64ee5ab96ddce47ca8cbb95f4f921909019c6e5b4ef359e9e779c981e6" },
                { "zh-TW", "784e4608ec516dfd51b449f55322284415b5fe8805508c6ee149d210e56f6ad6d51b3c49ab2e78fc8fca61a880e43a0985150df338ace8669881a8f31796e73b" }
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
        public string determineNewestVersion()
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
