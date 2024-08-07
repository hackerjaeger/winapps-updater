﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "130.0b2";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "01f019e0b2089520c201e41354711b27615dd1f9e35011cf767ba34b747b57ad2feeffcb81a20f41c5eecbd70aaf06fc8aa59b8aca7942a551abe17499953a72" },
                { "af", "5706eb03f831014bf5d29c4d7aab11cc856058fddd97a1a963b37af8ef766a8c0035cc984453c959e177338ab1cf6f4404606f7519992cacc5550fbd7cb307c2" },
                { "an", "fa2cd57a27c838dd57bf99aa56b1ba98b302663f9a1a5abfac77d3915a6f55882c22ff837bebc7dcd3aafbf8d96a675dbf86409de00773fba84c4a44a0130960" },
                { "ar", "9e3699b81d28b442c49a354970828ae7a9913639f2f33c4fa1262914f2e500611d5dcd32ed405abcfb5e52bf838af510ac7cd798ba37802afc65f36adfccfa3a" },
                { "ast", "d130fc7354b4957be8696aaa9a60cf07882bea209793e8191ef9b46265f8ec5d0a30fdce1b7cb24c4afca3fe56f5a6bd46ad897a615580ca03901e6008513883" },
                { "az", "1968b30c999fb73d4c1eaf051ec59de50aa63dea20023a08d5ad835c04c48a7333c04c58cab4b70ca616886597b87606474b0486ea7aec0606ce14c1ab8474d6" },
                { "be", "d418f6e6199d58a3bb51a3be5d92d26847f65f29c9328328963e90742e0d1498e8d3d320752d88bc91658434c019c762496d672fef4f298699454bd40ac3eb40" },
                { "bg", "8b8106b6aabc13f7b5c333a538d2e3590a51d432a09abacc5be6ea52e2c0187c7b8483968eb1c3c58e9452a337418e7e6547d5b05fb0e0ee71ce6bb9e9dbaeea" },
                { "bn", "479dc3b92e177d5b98c005b6c9595f614a346751d0101179c9586e91c1e5af6a13119845b90cdb4fe13ebcfab40100d7e747d7810635c1ca63228163453526d2" },
                { "br", "265419310eb569963b11bca4814877d7260d3f600105743a659e331f7f4d4b7ec3dd381dac285fc0ab0d0abaf6c87a89bee5c12cc26dccd685e358ce36f05665" },
                { "bs", "aee356e9c430f58fcf40079cc052546eb93ca6a31d20ec0513a45189e9eefdc32bb3e1b32f6090f99f5c61d00291f9dba86334e07646e5ba4e06973174a91e1f" },
                { "ca", "0451da35defc90264a7524078093d1f4b5a6602ec8a66eeb0beca01152dfeb990365bec3a66d5add05844d5d3c15dd915440d110183d6a1a44eda3c3e2a6ea73" },
                { "cak", "f0163b8f33dfb8aaa18bc36c61906e51b8eecb0153ffefeef60791773f10ffce66fa5b87e2318d67364e03d314ff1e33bc126deb36170c68c6f5001c92d0f428" },
                { "cs", "910339c54b17166506884e87b8bdfc0f8236fda4182dd736ed927824f31d50c3981e3ed029d45c2d637b2f32689a1dcb05ae596b5c047ec3ea003eb46d6fa17d" },
                { "cy", "088d4221f092cf5d5749f02eecaf7fe0b4ad135cfa5cde4d9251628123eabc6bf883f3ade9f7fbea04dae10350151f0c86b8c7a516238760811ec31c29170247" },
                { "da", "84fc17564a76f991970cab982e09f49c33320a49ff47740ca2196d1bf5409a0a7d5ab7e182d5b903673283bc4ccaefadfe81f174b9536eb00d18914416d702df" },
                { "de", "ed9a49123681d23fd934fae1b0df3dac081804615874b9205f595b3a62558f536599d840b3737139a246729bca9a791498bfb935ce91756c8e92dc63d43fae38" },
                { "dsb", "ac2ab64f8e970855c594e41ff4098fcd5b413e42bae8d6bf715ec4f0f4bb8f0d3a6526010f2b816d4c3d17f733cd4248cc5429f595067ca778d2c6225be11977" },
                { "el", "8c91223cdd79c87077da4fbd016224da672685304cf3e377c5b1bdd8fb364efff13ebc4b8f9f3a11b73d0670485f52e72b2233500231b51fe37bab472172cc20" },
                { "en-CA", "3dbafe594cda0a23f4f4e785271624907448e27ef738c242d62f155e12708b0103676ccc1d84eba5542473489af9d6ce90bde0265dfb6e7bc0b83fc8f16a7677" },
                { "en-GB", "2508ba7657a502c57f990637024e3cb08c8c5930e6502cb2838e93127ed47f820625b54299426639649e8f79d54584f4a803e847888b8ee42e72658dad267ea0" },
                { "en-US", "379b0cccbaf962727032902ae0d36bf468511ac7ebe31d4612b87eb1fb222e50eb9a9a500196d09b9a38fff9d4e57a1cab2e8e86606a2aad23eea50c60c7c24f" },
                { "eo", "9f5f94dcb7ac8a1352acf43149feb540ebd62abcc418c2fb592539aaf02d2dcec906dc51e8f396ffa2e51df4cb4d2d858311fac88cab13531113a2fa8f4fe572" },
                { "es-AR", "6b86d338432138c79bc217357979e27451801342b239841492b596c29e355e6b1ab2ca915fbf026320d6601b1bfd1abd52a956d0076b1eb3edda57d727f55691" },
                { "es-CL", "0c77ce3d45b0ff7563487aa7ca9f4c80373a84a9ed7a028b3c8fa5d154cc9fcfe71def8fc68bf1d5bfa1b706d76eb424788df6bd4380b8093580dbf80000467f" },
                { "es-ES", "c429f9534f06467a69d2e069941e20584921f3d5f2c281bfafac86974fc1d4759b8d6fd07baca0b97de1f082213b1027cdc033a795f83a03bd3e35f1cd0bc17d" },
                { "es-MX", "2cd0c765624a296a4bbf0ba47a67dd7acfa5acca55eccf5baf1254bbc154bf1c379c37371a69626b59678dd65f78dcc9f1bbc59a1cc8fb8d2043176a34fc9342" },
                { "et", "b471f89d0c2db5a7415cc404e191a55432ff629b778f0cd76d4d6321f21c32ed9210cb4eee981c7d561ec554307fe5e33e51316384148a7069731b751e693655" },
                { "eu", "a325a21758274e5490756a0d3a1f881fef03b140393327bf31cda0f1cb42b5fec0218e10c75c8b86785072b2d7a34f33f8ed755ac971fa4e7569744484f9c8f2" },
                { "fa", "a3bdcd9d8adf3840753db032050ff6f8f883b45fe9000d1e6905b32f287fd8ae3219288b1e81af6e40a686ad0b08f03d73c8ed7958e95c55358d8cfd9e92c596" },
                { "ff", "7efc996326fbd901f95d16fd78a2c40fce2b423838060865d98fa03d8cc957234d603b88fdce276bcdb424189ddb9b8d35573040ca4bc6aec338017d2cc47d38" },
                { "fi", "9e0ced9ed056d8f9f1e209f7e54fa4bb2edef2de7685ac268045c9eb63785dcb0492dd85e20bc60dde59a0bae89dc19124e5c8e8829ecc4cbd4caab307642b9a" },
                { "fr", "ca12493a5b8afaf7c59f2af53c3db0f9d5a58b1b79b6a543cd5369313bcf28e5c5590574b3ef38842f9202526da816f64a593418bb5fb9eaee7b3e315a95babb" },
                { "fur", "77b55c5edd158aeeb4705a8f411c81e8b63e65dc267605f759435833bb7a3f151aebdc4ed2ac060be1c8b9f8fcbf5f42d27ddb5848bba6a2b74982a77c5dc79e" },
                { "fy-NL", "30c8bb6e8a728d551d6d39372f3f6688bbcce01b80fa1e44205049042776f6a3cc27f465a8ae31afa79408871d6a04102af7e0749eff57bd26ba7b75f037c7c7" },
                { "ga-IE", "c737185a020c8c2cb40dd8ce75c4d9a2da598cd0770a12dd6726b89c9d1441355259569ae0e4d953dc8f0e236b16a8228a9e855c71035849728a2f31a37185f6" },
                { "gd", "221a58db41cb220660233d0a46add8e3a71beb6b4ed8b85c21a194fcc93a1d2aa42c378b1c06245bec5eec9e0eb7e980fdbf9bc319f001f1c2537fd9f7456800" },
                { "gl", "01981f27279fbe2db183c5e5ae56ce9c86ab30d77b71ca1c73d6451ba9a325f9385c28041f035fa884ebc31d411482ff544f426e847fe6c4cc674837c29cdbab" },
                { "gn", "767249088339d8169a067667742b562b6ca0ec9c498cd70a4bcae9b6edbdf58b80a93a06a65b662e6018e88cf5a5195445889d6ece80f49bff50666137af4b9b" },
                { "gu-IN", "d321d99469e01d4b9b7a30e777cd07912ba8cfca4c6d66f5bba8a76237376f19904add648eb517404a608ddcfc76c64c4b278cbdd7ce901e3dc3333a2b796d67" },
                { "he", "0bc3804a6fe2be44a9494ecfa3874282b467b05a5b872e1d687ad72723dc723d6b64192a2aebb447841ca42f76a123a044922bc7132e7e241e9e963a63667459" },
                { "hi-IN", "aed6c183e7e4358f57ba1c7fd4e25fe61926b8dba68ded99fad207e1c3c1496ca84d1cfdbd36882aacfe59b224e29beb88a2e8dcb8ff355076673dd48923f5f1" },
                { "hr", "83f235a0c4ca4b4ba9bc407d1546506afd72065dc405425850070a985ffcf7a363815a0262d669c0dbac755e7271523bbd13cd22e171448e74e33e6420563896" },
                { "hsb", "808acc1df3d0db665fb156f13c2bcd93ccdd5f66f7f0ebdc7039744d70c66aa90f1dba214387508c5c5e6858785727225d4f4b96a8c030c5c3ca3da25df714ea" },
                { "hu", "50947b3bd68d8625f1ca5f281d5aefc0591c1d91530566af935866a1965a7eaac7b94307f1f4dda497ea35f14af1c6f55d5aea082eee0ee875bbd041ce28982a" },
                { "hy-AM", "91359c96c411fe93f1c1f9c6495cd7fd11b49536f2540ef484c9c04eab69fe74233c4885dfbf1e65dd3ca75f8c0538b3caf25ef2cb1d337660045452867a43f0" },
                { "ia", "0c0d0a83f7f068bd3d1bf393c94cee37120482e9b5c64cb282e47e07f60c0fa9e7d693ddb752d139ba000e145f577d198a65fe4e1973491c2d91c2064968a7b8" },
                { "id", "d6116435d8d25b5d4a939a83daee6eac655921b1a5853634cc6664060fc6c5c78bc6196fc33d13a75561268782935e529ee0969825532e434a8187ec7a1df7cb" },
                { "is", "faa05ec32efb1278f809e9c67467a07c22d72e28a5ddc59b211b312a0737c3a38f9f1015d2575e011eca4e9cdd088ca3341642afb7731578b39056d0de0ee06e" },
                { "it", "08b4999238eb65a5093ad20e2b682a3afdc8fc205fe401342416277f2f27ba3d2b26cec6b6d5959cdc4090e8c2464abe3f5f70d7754f77a0b0109f7a44753d0f" },
                { "ja", "fa97e527a5802e7e4d9759c6a58115932fbeaa813ba23e73293ab38958faa81aef3df61170414faf546666b38c58768e5287f61e5c249c63fdad59920257dc07" },
                { "ka", "28ab10d901c6f8bf5bb8b029bcde0037a44109e46415c9d3cdcc49604e1b2997c1ad64e7655093daf4a1d0bef5991f0373c30f155037a3153bb011e6cdf7277b" },
                { "kab", "90a6ec5421c28319a02a2c9966173bf9e502f369bf4329d48d729b44205697d3a4a2a6848a3e2d7472576651e3557090a91a9956418adebfc1b06d121558632c" },
                { "kk", "234f5630d32409343dba8137a875bb808a811d716ed6a3c0f9fc001362e7866fab6b2151ed6f8b5adf471c7c356757c614de575de12e75aa30eaa7013939d518" },
                { "km", "c444434398f0f618943164a48174925296f0f1d9f48a4a43f309f64ee13e288d5b196829615d7fb34e5fe9d074a4c9ddac33408ed0c955b9f0795bc7c5a44582" },
                { "kn", "3afc3599e5547c55505e22b76cc961ca464020823d87ae1f3ec0a32a1ef6bd28338b0b0a2dada10d49f5db084acab57c57f71cbe71075cf8a39d8aeb9333b2bb" },
                { "ko", "89e937d679fb7b44495b8c4a3bedc3c39bd57f9adb33ca8ad19d6f4df50fb94346e9622aa0a5dac11dc9fccfa152c3f7ec1fd146611944abc3e6518f96c10520" },
                { "lij", "0b42ade19d2ec7b5b037bb17c3652d8c2497f3bf3260b0390a9a589a20f03bb8ae768793f44f8c9fe477f01caf3ca1e52aee8c4a314636ac3f99eb2204de8bfe" },
                { "lt", "6cb5089fdb8d49c34b81e9b3032024ceac46c6d6a3eaf1268e54ffbfc0a76fe710456994f8436a699385b4968b320161ec37ebf116b0e2919a7ca3bd5a2b7d0a" },
                { "lv", "52ea080cc511cd9f56097b0510eeb06f09faf2597f101fd871e0e2f1e1e0e7a9d69006e3b94d33f23b31d881b0f8862bb1b091142328c0c7f00359cb5d0c0029" },
                { "mk", "6f6ab6abba4386bf2cba103de80427c9c4f67369008c573418276bcf3c09f02d7d09722924b47d8f05e69049b4c4e8987f9bfca15331c2385a3a367ce9ccf23f" },
                { "mr", "355ea243839603f46018d2f506580021b494285771cec393a829662c87ad47129bf70ca972b14fd00530796ccbb279d21b59b0dc5b292f799b31811def9e6aa6" },
                { "ms", "b7854fc879e0889984581af805a1a08c9fabceaaf6a6ed30e1993a4679eb3e52364f5a3023d6e1fc2645328cfaf992327e708f9253394d67f16f356210a5311c" },
                { "my", "b250dc189de88b094418b80da974cbf5798ae371c716147b2361523bd4129dbf1e64f6e95317eb5948de71ddd19c92bd250bc87c8880ee06871beb8d7f8535ae" },
                { "nb-NO", "0c9af6bca417e66f6919508d4b05768acd8d5e46b1307c6cd4a5eb55c09ffb9fc4b174980480cb7646ae00b35af93197bb90e655a2f4d766f64297e32f8ec3a5" },
                { "ne-NP", "5f76e5da966a71e8ee7383d5e129d6d16ad5e2a261919280870c1eec398e0698d16825d198f4a6921e42b73e7242242fdc3cf66d14c1bec695917bf1bd0f4e30" },
                { "nl", "5c5c347551e091b631ab44575025ee03fc3d21126a3e06403c1aae868caa8aff634cc224b0a74277369d5e99b22f4aa92ac60475d88716e4e7fa729301a4269d" },
                { "nn-NO", "e4214d8240506ecc3e38964bf80eb98c3e28cc49d4baa465ad52a873635ce6664bf0cf1c1d65c701eca2e7ec9f12953b2399fc36f81f16dbed3b647922a3e4a2" },
                { "oc", "75345d2d0d56c2be8908ca62a0521ea9342ec09e467c894ec83fb3cfa4ca405f424d50e849f967e4b6d2752e6a2a078fa798b2370b72f9189d0fda19e5419ee3" },
                { "pa-IN", "a32e7bfdaa162b7663b88190a2d4773ef4c02d67b7667ae4c8f873ccf46e7c5b394cf52d3d2edf2547d3ade26f1e863919b62741c5c3c5f00203265389ba5942" },
                { "pl", "1fc160cdc6dc62d5addb4e2e659d2e81b0b86601ebeb82c29fe8442efd52ea58cdd1c880f9afb53b7bc1f0ddbfb7682be7adeaa4b580b5e18132968503455f42" },
                { "pt-BR", "e85a9e8558fb58458d8a7ff6ab2f68b7fe9f295dfb9cd213a7904b2540ab85de81eadf7cc24a2f63f29ac0d8427f76aecccd057e922e261d31cbd189265b96fd" },
                { "pt-PT", "61f81dc263e42d2c9454e02a0a44aee3dea1966e94fc3310605a3eba623d2e79ecec81e98d4f9b74676b429f3eb193f39680edc06bdd104e83702b129d07d0ed" },
                { "rm", "9fc3e5a11dda2e504378e40b8328de7ec0ae117041ba231eb904c0e856e0ef3fb6c325eecc4c5471f7df115861ce2f9b4b3cf72e48fe0af63efd7a7729db3af5" },
                { "ro", "403162fec4c56ce25cb9598234e3221c9bbdd92faea8c3629b05b0549bdb3e642aa7e0830051bbf3212e9a9bf478230a236c6e79c6643a85a8c37bc224bd994e" },
                { "ru", "11a586dfba9f423a09db05dfa285b94fb5a4ceab03f2c98b13d25a63951f967e60148ce094dc76e8252edb39d53ac933fea434cde905e92abe933d5cc7edb4bc" },
                { "sat", "425976554bcf4a49921625b1c67a3de401686a674da0bc59276bc64e094c29271ce3658d85dc6be103e04a841fd1fcb79825ab4ac9e42d13b5ee4b31a088cf30" },
                { "sc", "3c0f2472244807cbcd82411c1c06d7d4fd1116f6526cb12b6a126c1e8dbdab630f418c9877e2e64ba2116b71ff6a1b02ad5d0dcaaeb321da69087d44cf60aad2" },
                { "sco", "32fb9cd14705b916a94321872a7aee28c4c27015899ad776fcaa634058075de6bc03bf4c73709bef6f24bd2691baa603bf090428a75e63070f45a271e720dfa3" },
                { "si", "313a0a83343ea149f18b5ef968059e4b4012186fabce0932496cb8066b390e220c526d951680723066ca8310e3c793b5877e82c6a82eb4169d813e289bcddded" },
                { "sk", "d7bc4da3378ae3d5d14bedacedb0078bcf4ea433b7981829ab4f318746459e8867123d1c849acccb42dd4a4d3314ecbbb16cf9da56900e4f310b2488d4d66a5b" },
                { "skr", "ea16451a440d9c888f95d48eacc2606710657bd0087de2f5d6c87753392d7c7f9323f4dc8558e7d53c0976e4f4362fd625abe93456b100d20c4ce4b29041a8bb" },
                { "sl", "1d784f6ccd07ea29a3edbce021ba324b3148b3300ea12b29f007488fe0a223e49540480ff29a8d010e4657a651e44a476c6e11ee3c54743049196a8285c5168e" },
                { "son", "53daf491e1bf98fd1648f18eff740b00053400cea0d309b0d1009e1b28e0b1e5771c5b2dc21d76ffdf8cfa40c21664aedfd2296aa9825fc0ff8d26447eebd506" },
                { "sq", "f9dbf33adea84f20fe52bb869fe3a1c7da328bb2ae126e7f52f8919454eff19f0522578fc67634bb6a468a8bb354401fada40822b60ac92a25258acceb5353f5" },
                { "sr", "3840c272c65e7ad16eb756f6481c2857fb63f302d5019f135230d249d040033899c7b25da44d5d2200e62191a11863e7882bd43956c1e2ff720ce5d212c78416" },
                { "sv-SE", "c52a16eb8c691ae1a3cc52f7543c2be4914c7e975bd71e91596a0960fecc742136568516884e43450a02c38e60a4b6b640a691d1e5ade244007602706f9cd6e1" },
                { "szl", "e76e30e0fd151da7fea6ec66bd0424af98b35a083685ca09986c2bd8487b96ffd8a33a75cbe3289258dee1e31d0971a341764c572b2e183896f7c5f0ca4f87e6" },
                { "ta", "a4b8a5c4d14371f2737f4cb3d6eca4c8fb93dc8e394bc7818b0a3df931816a5e227e1d301af759ca231ee14bb0570c5d130bbd06b3a33d0a5307a4c8382099b0" },
                { "te", "5cb52a4eb2675a03df582025d6285b6606833046c53a1e5ec7de6adc37cc5d69e8847e80abba2852ec9037a40e2fbad859ba3f2c216c70f5beacd02b37440357" },
                { "tg", "b53eeb0572d10ed71b9c945855d893c5a2e9b7248c1bc87c44d3960edf6fe85739a3a6b2c03cf8d42d3cef5eff6616f201fe9ee59b1d73e9e253aa9821e1468c" },
                { "th", "e8750287519e4f5a001b52a19e32363ac9a1f998d25ea11ab509544fd212e973c4ca5453f5b1d60ff3ca189feda1bd3a018764cc76694bc038cc785b63128477" },
                { "tl", "7dd7e67e114d3bee579c0f24da1c94e56ff9e72c050546d6080f91f914f09b4431a6ce61cd691fb07f577f9bdbc7936797179fc179f22418f0bac7a425b50a5c" },
                { "tr", "2be87d8189dd15b6ee76a7ea2c3de5ff5c03e256e475f0f5f911993093639a055fe615e35e1e3a6e9049f0535460d408a0cdb2c0c6f98d49c8c8b5dbce18c758" },
                { "trs", "10a5f445ee5796e389b4afdf9ed1cf7e56c4db12216aa819897f80957e45a121d857733967f2f8173c4d6dc11cf9744872987dc8530c396c8fceec8bfca3c97c" },
                { "uk", "f510d8bc2b9a16de0b506833036e140df86aebafcb0effa992d41adb21e3e5c4a5a656523283d8d40a1733782c9b810a8d51eaadd528c106f0d646b4cff9280c" },
                { "ur", "4a59056e5e45c06f37f446d89d2d617848f8d65ee17759d3d039448b0eeb152a7e2495992c92808b99cdfda521e82383da19308c95ef46dc0d56748b4a58919d" },
                { "uz", "3d62ac324d97f72c2f25cf3da3c0cf44a602603a87a9a2c94a1070bc0ca63de6a41fcbc66034fd46a2aba47b91b6a46234826fb309196de04f268eb17bb1e2f0" },
                { "vi", "08371fdd2c61a6280bc6341a5d3ea30add6701268e619cf47178be349badfd34060ac1be2fccf6488cab4e8e63abe1f325b49fc49bc26a3b98b387b94fe7d2d1" },
                { "xh", "9c1dc3aa031876a296674499c12ceb9f33922e0a1582992d1f6797f93f6c1ae5fad72bcd9dfe8e576a36daa5160b9cbc057176339b3143e940189457e60f8320" },
                { "zh-CN", "e87e5adbcd103e14f77840b2552754b618679d59ade3d06869e6172eafbf640976051f81043d53d8db040f1cc8e46a334873c4073f6f10553aee1e7d37fd27c1" },
                { "zh-TW", "17427a76a6fe9fbb158a54033bfab4ec520faf1f8a8ddfb68cd7941a127a06ab4a132f83f1c14cecdb31f43eef0238ff9e1c33f26fe8fdbcd2673416e0fbbb51" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/130.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7ef5342e432f624eee337835f70a61aeffae263354946550ab4d280300fad8f3d341c67dd93f63bf384f0da2e223ba70fedee98676fa0a1a0cd6dcef707c1d53" },
                { "af", "54e3112a4f596ac52541eb96bb1174540430e7041875bb4c7c5225f8285bb1045f5b13a550ce61bd0659ac383b2fcad425083ae440c6d1bc55c3b8115e61cd6c" },
                { "an", "82bf0af5479486018e90916951f9c999b8f5f3d2dbae536758982541598e1633d167a6d0e0449d7d27f2217a6138c399beeef836adcea509e41cb154eb4b402b" },
                { "ar", "0623221d8ae421872ea226367b8344703b37204f1a41204deb97220886d170c7df348366bfe8e9d494626873223837d7d9e09c2c906c824c379a64ca9f3cfd20" },
                { "ast", "3c96870009c1c04277ca4222759107327cbf106e13f076f3eeb1bf82d0b624a2e53e6a7a03520f66a623c7e4d4226a45f52279645c3ffb5d4e0edcaac3b3490b" },
                { "az", "54191bcd9c690299dc0d498ae8b536cf5d327b72173536fa55f49fd7fccb6319e40d484dda394de1914aa4bedee966cff34c65976b27b7e313a53fad51ba1ecd" },
                { "be", "8281c16568b040aa9c38073e5ce3a39c5aaf39434b388472abe2ddf466c4653895eccec5b014a5b0f4c8ea4122da0cfc7b97e6f454479099d822b60469852b4f" },
                { "bg", "8b91fba33c88225193fa3cf3d04232767bda3e01386075eb7461223485b64a411221c0dc7c04b13d59cdfac71d80c5ed1113c7002c2b69fed1794f277464203a" },
                { "bn", "c26218e36aaf33b19ad6fbe3324d23e02d6190c2c365c31f311217585e2d4c51c388c9cb58660bd0831f9b4e3a3048ccdd8aaef7bddc85c102de7c65f53796bf" },
                { "br", "cc3a9c54da9b78b8070eb9e2518e13e3d0b1c2a4790c6f191ba4d3d57c832781e9b9696afd76f534285bf762c97e265895c3c354d608efc2ad344c7b4926e28a" },
                { "bs", "e3ac758935cba0f700715eb4f2be5c94f9ac5c75a6b9a8fb8d1622ef681f1930e7bd7744a9ff9ef4d1ba45d5727dc1ac19a35be9e718fd7ef14029263059b84f" },
                { "ca", "e8eeb7ef5f975a473541f37cf09f1c4680d0fd8762f0d1d9276e02b54e6f90b8c1d95c7a45fb3f1321bbd421581aa4f02d9c7db8f94647f9b817f74feac56399" },
                { "cak", "0f4694a476701fe410dcfa9e92e838db74b556ae7e2309a0d9ce26ed4bc3a26c60a52e4072377bee573884d74686b5bcbceb85ae5616712ac1d5037549232245" },
                { "cs", "72f806f953ddfae9fb58994ed3809ef05ad3f86bdff2871f3e61495403eea6556aec2b4ce723252c3379153ea1fd375d117a61c92bbf200ca48fb2edce4b12a3" },
                { "cy", "d72fc24085100fe1f41f27151f9da013534de48e0f05c3d719e3bc006df91bddee55e16355d94450b2715255ac0a21f3e31b0c7029ce90489bce47f470c5fb70" },
                { "da", "8eeac4d31bc576aec0de38ce73417c5f5e4bd2813e0b1a3e858caec0e1687cf4bf2f6a78ad91eb4613fe65975067cf6e83fe2e79b01ed47d5b45fb660f5fdb31" },
                { "de", "a1a05e56c0d9fa4e22895f15ad437ca7a20d07ccd25f54b9fbf6fa6a94631f14ecd65d1781f577f7a6eb077b1f078391112bf3a488d301a11b7f7c585b14d49c" },
                { "dsb", "ddfd6cb30776580a61d3e0f1193c0449b9a0921293c7537545f042e8fb668ba3353d1bf935278d88946c9383e5f3bbfe63dc646848e09f1b3563aa782ddbdb2b" },
                { "el", "2556d905c10baef67dbe88dc637497a0b70317ea32fa553cc4be27364a435efca2cf410a2fa29dac16e37612d2360df728cfe05f090ffe379c11c85855d43f88" },
                { "en-CA", "fc3c408b84b82212a9524378c1349bb3c983fcb4664c1ef03f35465b958fef60d81da765af3e3fed4aebb1947d0a79c6969c320cf56116d7beb96e4886d60397" },
                { "en-GB", "8dd7039d14ad52767c1998f114b9a0235f62f060ae31b60fd80016854460a847483954411440556e3e1f326ce8a14e332bc3879813d8d8e4d3e4e56710c464cb" },
                { "en-US", "20db71c52dc2d7dac60c82eab148a9a27f38341d27f8157639e156d050c050f639ec4d49f85bfb23b0875400c1694a4cec33c227e43dd9bc70ffefea2ecd890c" },
                { "eo", "1c7348646492e6feedf81562fc116577a6fce7a1fe9a6487143f2db22cc9e0a5e55da49bcb627761aca341458f80954661a9cd96dd4c561cca6d19de738411e6" },
                { "es-AR", "665141769488f316f76da01efa86a0dd40c5e4f4d724edd0af644a97787e99aac4327d085f1bd9bb4c8b2009153a2524decbba25418ac7aaa50655ab110d9911" },
                { "es-CL", "e269dafdf9b7c75a7654d8d572031812cd84884e72dfb87e0ddbd5322b520ca11cee65113a335ce1094503bb9700a9e08258a390b58ce36d828c84c00209bfb8" },
                { "es-ES", "506edd1b67a39ef4151bae37e8d4ae93d3874aed342851f5c14a04741d177435610647fbb81de346b171397983c59bdfe1f4e0419424b3483417de809511b417" },
                { "es-MX", "f39303e47aa4aa633a444d57c8bcac4738ae2e9173683275045f756d67ead8fe09d05082bdd8de246d3ecf1ec5ff6d875f5c68712d85f7091c631b6b7bf63e07" },
                { "et", "05f95e71ac87ea0fc4b15f14e1b66303e10f16caf0c076d110f1a94966caf85f059b9d581bdc89cc867019b051f45304c3aa9700a4e51f8c158e823733c18770" },
                { "eu", "e2dedf9bdb66f24506920102b2edb90233759e2acd09ef16dc0492b5941b944a558056a78765322df7acd26be6280e6a19938901f11fe18d3f2b067975071269" },
                { "fa", "f123bce7461208eab30f35ffd35a92040e00d507e8010d7944db79efabe0628b5ae03670d134e5a00bc7deefc15dcf78511636c128a5f7ed2605c9d132f974c9" },
                { "ff", "4f38f2a9f05022885629d8c10f92b9af6a03318399700a84f637452af03ebbdbfc8adc94cb08a0a78def58df0a6f558acb7948f72f803daf0cc37b22005b649e" },
                { "fi", "381919faca08506fe11d32915c1d12aaa24df013d2580fd534f5c5218beb2e10a2254ec195f06a09d6fbf35ce35737bbfa345dfa51f9dbf66e255213e90fcfcb" },
                { "fr", "73e374a39cbf3fd853ec23705c92d264d4990140f2a8a04b04c97f7ec00eb9c0cedb20e80403838a23c62be30db11f29efa64cf6e22bd04fdc3dcfa3c95868ac" },
                { "fur", "cfa3a2990d401ad34b85146d742efd372c7923157fb2a8bd4a0c4cc7b38dbd05e324a7e2e7f5ba58f3496f8fec713eb2abbeab67f5b6952c439877b659f98343" },
                { "fy-NL", "92cd362f26eda750f375764fee425df10ce756711075791281332e9ea22e5f93e5df45b07f637feb618cfaab6ff1753fb860c410742e92541c00d1c67281d4f9" },
                { "ga-IE", "97617e25291d25a947b1c5738656f14f864fcc94085a090583d1289cfa9026999bd870b3182252ef2a351e3303ed622c0db9dba0553a844a175b453b07042651" },
                { "gd", "84992ec16c04c68f659e9e43e7ca19510342d09169223ab99729975dd495e5def7b512beb706c3c699b67033586848666707bdf994a98829b98cbbceefff8889" },
                { "gl", "822b9e52e708f585e7935cad4a5bc8348eb0b098ffd9b2889c3c8e2115f5848abec68fcd1c2f50fcc65fdd1d2aaf54579e374dbba614308232a27530132a6e2f" },
                { "gn", "e31692633454d9e180bae2aaf3ee912ce22805f8d71f4e1a4490f344318091d3996d728d119378dafea1a9b45c7fde71dac4efef7048023079ee0e962be2650f" },
                { "gu-IN", "c2638401200105d897c685bbf775799dab80155f496b3faa50fda7a66f959d93e57c89e5041046dac0e5a901d41f7cfa57ef877f40c5b37ae2f7995c48b37fd4" },
                { "he", "b1b8cb52cd9166540f67a5ea7687423794959cfcac34e279cdcede0c57d35624940eac8a5f02485a4e8cd45b352181a13646787a689f4d4732f8ddd3f440731c" },
                { "hi-IN", "31a511b2e967fda820e337b277b29142bea840ed908ff4a0bbae2906776bfc49f6fc129dc5ccc79774fe2c3583217289073394457453120cd8ad422e7680e259" },
                { "hr", "08e64e1535f65c2384788943c8ac39cbbe7b627aef2edb9e1444bebf642aaa81213854ca4fd0db44cb8786f9a80a37e97a57f56819a23dbcfa05f244b32ae7d3" },
                { "hsb", "ab7a704f91c68fd54157977beec8516ebc34905d97da733a01c81dcaa8417d546a1e245a09214f36535b34deafbb3ee2f192a6bd2556843b33fcd688d6d2bedc" },
                { "hu", "3e22ab87bec5a492b62b67addd110bcee595a4f2c8f49b5d44a07934b1a70658dda2efa09cb3c89f1ae1c36e09543190eede883cf1b5a692fb77c21db027e43f" },
                { "hy-AM", "77098eb3300849a5b64f9fa153e14e2bba99777f9ca83c76d3292c9ffbd888af9a3edb17856d6f44fd3eb442e523da0afef51f7a449920120edc560f2042dd57" },
                { "ia", "c785a66db1ef4827c52dc38ec145b34a323900f0e0df7a23aac82e319ca582613b5d1787790351bbad2a7f84b82a54213bfb959cc2d638fcae21a499af6f6328" },
                { "id", "09cf19cae0b9e7a9d1554515c037436220713b4c4bb89b1dc1d995bda266d6d47fe333ab0fa670f3e541e8dc78fc68750325e762aab60bdc422202d6ce7fa2a6" },
                { "is", "6d349ee461a5d483a56ddbdb88d3a9b9135e3e6e290c7c47724be30a1b8d3c354cf1aeaca0f827ce6566065415ff985dcd085af0c62596bccf4710b6ffc7e261" },
                { "it", "0437d78fd5eb4379d552ec5342ad742733ace0e1d2f3efa23473e6ee52c281f336efada8d2132bf80110ff1847abc21a795fd6006acbb2b6aca9bbf57637eab5" },
                { "ja", "2d6ba97aa9da98d20a11c2ae87eef5a8bbd248fa3771d82fdfd92f582f668e5939f801f59b52e25aa7386e674e25bc8c00dd108ac8c2c287df19e77df26d6c7e" },
                { "ka", "d1313bf81894368a35c158b7b8c4e25424e6061bcccaef005e45594dd8e976065c5376a9204009d053706aa0dfcc3ba5ad9e2b3aedb0ef014e7b733af7d83af4" },
                { "kab", "fbfd09150ec306aa1e8958598cd9b5865d2045ff3f24fe853d2295eaa39f83c092b118a34a18300131e926cf40b30dbdf022302b76cafa17c486f0ac41c76638" },
                { "kk", "21b6cefba92499ee1a79cd87ad1c30bf91ea17426192d8ed534f852b518a60a92635b2e0faed9f1d761b44367038bc7a0a90596e3d42d7a242d8195cb14f1591" },
                { "km", "f42a3cac95432cea01a992ac02001bfc19d21b67f9e1ad454e842fa07810d8f74e6bbd775195d18eb6a9bb6c533c3c2371e3e0251ff494eb63eb2acf151488ad" },
                { "kn", "eb51dd444974e2aa418dfbde69b081f68292792097d84344c2c39ff626e90bf2072ab4f1df55bfd17f16834917e7a32781071c97bc1d6d43811b2b3098102d9a" },
                { "ko", "ddf8c4b5b50dc1d10d39dfdf9486c7182304a7f894414613dc12d06e2499409c3886afa93899a184ba950cee017f1a9a143f8eb77390f3b2d0d67de3cdef100e" },
                { "lij", "f78c98041ea877c281eb7ac893192241344005f11364774cbaa9d1653faf9bc90d31359320b1a0d3441bc9af3c4b6bcf5f0ab0bb83a89acee45df8ccee19e85a" },
                { "lt", "86bf9c442bba518607421e1218f00170109e004dd9f3b75df661a756877468c08d404b6a558be1f702273003bf31e249eaad85cac394b2ae69880570f69cc8c2" },
                { "lv", "96a583ac0ba96a6a9876c7334a5cbd121a340ef9e434e0b947dd27afbcd4988af1b0bf14d60b98aab17b10a2856eac9974c07bd8acd338e819957459d09218a7" },
                { "mk", "d126c8eda464f8559650d420c183328fcac2bf8d3d262570654863dd346fbdd3f7bfe6b4c0c595430d0ed2f4d454e3f95d089c4082854102a50a3a534fe0de54" },
                { "mr", "f2a38b80ee6c20932e8204f610b49bb90040978489e948eba3de1bc512290f395d56e82fd60aac54a12a4389e609a5a58056f6e2ef5fb7adecdb2d94927a5c47" },
                { "ms", "db46acdbc381e753b07e0bec87f940331c693fbe6ac1dc60aaf4a5a1859cf4de1fc9a6b8115bffa3bd12f7d99d362e14c084a1c13e332999bd18ce91b40dc483" },
                { "my", "3ade94f4cca8ba13691123efd1e3f1a8397b67e6267a344e42fd5a8bacd24a2b8d2965a4d3df79ef8c70666ae5241408299044564d06d16afa8d2bae0a14643f" },
                { "nb-NO", "fa626cc992f1b207b826470bb5d12b35c2a1d68b83f6902a6d613675f9d8ef8ce7d583bbd2af2680f30fd7c2d907e69bcaeed3c73d97d1f02ada08b18944f838" },
                { "ne-NP", "f2097c0cce07224f7b73f6e0535c9604dbeff233b26a93f3e9c721d339c56f08ffa3bd297fe68b6bfda25d4a767e0b69ac034fb792b4aec10044e9fb933c46c1" },
                { "nl", "6ff5bacc44069e9022b2894fd648b97b2463ec273f591ff7809fa14cec711e562cfc46103bc74e7184d03c3fa8b3e3565795a4067936e7dee0b138f5ebd80e3c" },
                { "nn-NO", "7e043ef956e62c75730e808e1223f0b5b6cbfe478bac8df1ec42aacf16abd74d880ea52b07eb93dfced5463dcb44b24be0bcc959b03b8e52525a8f4da82a7c4b" },
                { "oc", "d69537fad7bfaabaca4d00e6835a6f0e6423a073ef6fba9628394823204f376d3ede17ce7aca09fe3db3e4677d8de7e2c7862bf0395c29eb2270b382a9b268aa" },
                { "pa-IN", "3ce016b80903dd666846df75701ff8468c05b3d44ab45f30b99624f1e2fc5aa816e12a8305645f63402eca2e81a766d39257db1773dc2e84fa7238de2b6fc385" },
                { "pl", "341e63ae24e38444596ff8e99f03412c41ec63e2915afc4d302ce37e90e58f843686317878a11f99b1a1b83ba2aca21aa4d6d9743067eae8878dba76dec9239c" },
                { "pt-BR", "af47246233ee8b87353674f13d3b1995599b90e5838115d6e3e7162d6c65ca6683a6c21e3002fb495a089d8edf862764db2a4f480ed7b8f39ad73547e9139bdd" },
                { "pt-PT", "0a18bba5711ec18c205880d46bd0113389f31d8d5258ea0678ce5751d05992bd499f16406a2192c7b31d199d74a2b50bc26c3c7394155dc6d6f07b1412fbd0a5" },
                { "rm", "540dcf2e28668080f99aba4254627758a69e3f454740a9052940c30ffc512d6b80d3f6772fb439e524a6ef8bc52916e52160852d30ac93cdbb48741986598c85" },
                { "ro", "b6ca45ad17cedb4719dea66bb50181e616dc33613a801b1fc5a70168a7d6da219185ab0ed22142b43133ba1d69e702f24fb73b693971cec5dd307156f8f9dbec" },
                { "ru", "7777bcfa19ef152732da0d7aae00fb41f0ca6723a10d51a5fe78612d78e82cf617ac805f1db0c93ed91771a75b326e52e4b6406305ef7c6ef1983920be729fd1" },
                { "sat", "656c8dd0ad83a92547a26769da59cced3e1506e9a0346dd5baa3e9c934cbd61b0d83a710553197208e8d50949c26e866a4d52d4ee33df09ac6e27227a500061d" },
                { "sc", "752419fe053d939c1a41a5e62cf9936facb0de70ef9a13f0943a77e42256489ba5ce6349bd0e37f4a02abae0132ffdccbd456514eb5507ca6c29b48e6d33a210" },
                { "sco", "0c4113bccc93338b582594d497de468d1d5e7bb64a51288469c6018369257a2b1b7b32eb8850cb96ffbb723a40da8e8313fa5f94b2868b82d5ceb057f54c9124" },
                { "si", "3ebc14c7b1bdf57e3613add437923d2c4689a770afc82f12670370b5caea85202c027fa99e1eed16ab20f6b83f2cdd8f8e3b8d285482d20eff449a91c05cef36" },
                { "sk", "a19eb79011b4e9c3b7ea401f1a08059186f8a9498c9087529b34147c35469b520e054bc22846b84c5a5ad4bab6d273363b2931b8b9b034e0cd6aeac7286b9e5c" },
                { "skr", "f360548dd05fc589450c2c4b4708add0bd7064cd5ea8d0ba7b352ab5d6d09e4438caac7d2dc54d8bd1eac8273840da16381ffe9b7aade574742a81e11973ec2e" },
                { "sl", "e0cc60fbfd662f3bd0f930989cc7168d1bacf2e2c28ae725b713da3aafd81040d33fa110268d63b346cabe2568b81ac9d41d16a2cc9184b355221028ba8d81bb" },
                { "son", "09f83d47a8c4a7373e450f7da2dae7860ac302ac71f9d1b190a6323bdf24931834ff90012869b47ce15001740e9b7f0dd34a7f65af5a6db08b672147057fbcad" },
                { "sq", "cb0bf0f13b040b937f1af6e8e591f3aa8d5ff482bb70242ac61a2bd006765383cfa70b2ea23a63aad63297669f4bf9ec43d1e1072ad290672a792779eac04308" },
                { "sr", "664bf53f62c406f863df9d60158436117da35c2d8de661feeffcdbb6a9cef816a8e210ddd1acf8713fffa1cb836ec579fb0d1685156dff17b67967967edb3e9c" },
                { "sv-SE", "4582f38f5a54a8cd488fd69450b56e164359149d1b1686bfc5b6e301dfc498cf69a2df8ef28990f123285dcbe4564dc9570d169a235b6e47e4690155c4a90861" },
                { "szl", "710ca72335219eea8669d30a0db88d2850c4a127728f99281a9dd6717c6cdee07d85d9333cbb30d302135282c66a9436630f80cddfedcb1ca782b8aabe3b70d2" },
                { "ta", "ea42d15fb8d751a57adf5a1349c575e27edb5f1f2e050c50992a12ec7ae7ab04b259d2e44493e1a85071c8f8d6c99f3beffb5594351df152f6f3ea2a7514dcfa" },
                { "te", "d966a7d9404057ec346908e7f7e1e29f4cfd7e281c81f6994bad5d4043ced14e729eefbb733ab60f879a8bcff44afdbbc0474570bcd94bc1788a46e4885d5f3a" },
                { "tg", "22c6296d4c8635b8251f797df4819f581a80a2844bb48cbb110b9a024759ddf1e835daa62d8bfbdcc6391e4b25e2bbf1e0d44a5b5ff6b1409488e14aa8206667" },
                { "th", "d5d55ae366699b2a72371a24ca44f3712fe5d2d6c8f79d196e98552ee9cf509b1c89e76eeba735ad533e968311df42002534271c86089beeed9abf1da83bcdd5" },
                { "tl", "0ea263f28d48aa107e84edd6bd65a243abda5ccedf718b6e5b68934bd5e52adc74fdef270dba3570ca7c19a60ae5af1be6664bb77f6041a57b63802b7fb324ff" },
                { "tr", "adcf7dc56c3414229095e7c416720a33e5a8fd7ede4ed2036734798f8a084d0ecc1af47d83253446bab710fc86871821b6f2d713b729c335983d05f54eb2787b" },
                { "trs", "2e2043766038b5b9c0e0484203a11112d6f1d2898d6fa6b1b9ffc6c14f9715cbb7885b5d41a5c283bcb1a8a87223bec243b8bbf50a8444b83eb9da0ade599757" },
                { "uk", "0489cf15ba12c32c45676d9e66608a8a5649d210607852584c1544f31dd2ac858e97d7d6efda4affa0f365fb89867964b0cbda00d8c4e2c760b42a194ac50a89" },
                { "ur", "0b0016290139ecc1dee2b9fca853e1849d5845a5bbd3d90e91d082c33673227a33491b7736e3c88c87a3e801733c66527d80952ef020991429c5f963a653ad6c" },
                { "uz", "54da7b6407934c88622b4b0e5c580871e93de8654c1e995d058a5f62bf39162b91440d5445c8c5d103b42fbd8321eabb31db2a66a4cf7c7a960801351891e439" },
                { "vi", "b355b806852147c2ec5d5393b6ba53730b35e107178a9fb80728803a4e93b57e3c66685d86bcfd8202788a573437882a254d3d86d2669a5dd0284a120c6986f9" },
                { "xh", "797e3295a06e8936593170b992c0ba7dcc659df7aebbd5b229f026e2a221abfd4fa8d7ee6c2b10a4a6503437cef85c94122d24e48d7937105912de5d3367c360" },
                { "zh-CN", "97588fc6dda90dc4fc7d2c2c5009b47780d7d294dfd9a561c45da0696c5cdfa65a3c2be640fc79fab5acad326dddc4d72846f9f324c91aa7d8a7af6309445eca" },
                { "zh-TW", "8691746926307b28078e14971c0307c40ce683251cf0cc487f2b58c81c37bbc0ae5a74d82bb3eca85c9d9c0618c445fd132267d14d648f923f6f92809d2a421b" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// Determines whether the method searchForNewer() is implemented.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
