﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/104.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6b7c1434fd89df3b3c6858e996f50bc59a0533031382f0a792263128d795fdd5de5b3d872e1d7542717d4a45ff0a013396e216499e1ff8c07738fe7bd670b004" },
                { "af", "17403e6af180d91af17a6bf366a1579550cb4514aac1619ab1bc9188590a3c41dec8831495ecc6becb6b74d8ac2309888ba2b51202ea9800ca456f98b989faca" },
                { "an", "7a1ffd1f83fff7870ff05844af00df45cb5babfdf4d71355bc2f252d653ec38350127f788104f2f4f6f8cdf1f53987d05a92bc0da304401933c92cc70972b987" },
                { "ar", "b7563deb3510037d393227de3ca5053d9524f35be0bdfc9f49348aeb899cb15789132b22cacbe9aae9505e17dfb3021aeda22850a7ddc56f65f73788a0c5987c" },
                { "ast", "c48e5740eee0b602362e59f47013121bb4a2d3ef3e970c66488aff391de959110af6c6304e54bd3ecd8e3c300e72e98658844385091bdc8969f8708578b83e77" },
                { "az", "bb846dae8c3b49f4e612946132c4486039c33380db38741e142c5791c99e37a91d65ba8144c6337017da450e58bc7978d17ca12415e3336a011fe978f3ded968" },
                { "be", "04e772f389c0f3c6d871d07057ff275e1958ebf88dd3fc5075c4b21ed5906a39ef7c82972873fb95b2c0586dad3da4df2345df5167b6e5ec10562a9edfbcb89b" },
                { "bg", "4ca8f15fc0c350b296f7563e38fab12fa6b78b7b762870f0638dd6c85b8f155164a4f574ff547f87366cacb7dd96a421c26c11f153d52ed8c9be2c1a608bd269" },
                { "bn", "b5ca51ccaeb6f2b9ca783d219a803bdedb0802f6ad122bbbdc21ce5b07a14247fb3a5af12aee69b9d5611d93f17ce05717f8e8bcdabc5d36bf720e501423e422" },
                { "br", "d2e6b8d8f6e0b55793d4639b39a035f635cad2beaf0746e6e04f78f6d09d3b5f3c470f099c9cadc44bfd35456204cdef0e025339bc9c3c5fa60f5838e580204f" },
                { "bs", "919dede2cff928281519fbfe2e147a237afade58582aaff441e7fd7a2e8d59b7f88229ea71820e5b1fae113d4aa1f6fb16796ec94bfb0eaecc0f18a2a6ae182e" },
                { "ca", "5fe29eb0e82c3625a6553257a9119508d620ce6fd75af5bc9801959fde2de433ccbd1d8afd442cf922aa6368d780fb98a2c6bf7da105ffc3bdb348f9dd1ed236" },
                { "cak", "47771f6af1921bffbe119ab70ecfc998b9d903251ce5e07899f6211075d1b95193cf2c6ddafdb8a6b6083e105e8bb2a9147065cbe0c1df0afc703a8764ecffe7" },
                { "cs", "feb970fd706d2cc3b7e9e792539644e8a867d503ed204dc467b4405b45aa892523ba1f1860c8ed407a8236c6d34d04149c194daa46ffa3add705e29a2d14eb3a" },
                { "cy", "1913e4b3b9956ad3f48aed2db972ccfe53f9d07426a4cfda43e3dbdaddde97b90d82ecf36bd340e7c7f0e7ac32cb791387aaebfa2c8a589b8f854ec01631fce2" },
                { "da", "0c3d4aa83c75c63831d9bcae21b9c4cf3ed34670e88816c5dbebe2f064fd1c5d7058e61f28ef15be92645807a94049b6d9d998d009989d135d9c2de46de25ce6" },
                { "de", "f0bce911b6df3df2cf7028302c6d0630bc77da38b23209d37ce552cb24d700457085a4fc791ab257cac1e6b4c654448b6aa0ec3dd1f252d8ff7a78714dfb9a41" },
                { "dsb", "c0f45b1d256e94adf3f818e8e2800ee7c26d8d07db5901486e695149e6e76ebc66d7c4fc542a33d34e43e42ec9895cd926dc54d8048501a80c6bda2c4ec2675b" },
                { "el", "60baad358f640836fb368b5fc2ef05d6774b43ecb94d71f7adf61b88902cc6f5236f0088efa04240f06fc3d89842aac4bc90b8ad91ef1cb1c9dea483d80357de" },
                { "en-CA", "efb9441c9bfdb00d9538d3749a2811fe5187789f53b7f951d21c349d26d3062a0ee7a1958946d08ad2118bd1bb00ab4f9c100ed1215dd8e8e161f1c329625b59" },
                { "en-GB", "3133ac9504bcf01af3a584ba808e87f22c3dd17147eec8d1fc69743698e8f706f453f2036e8551d390da6ca112f71526de78e5296ffb8b1865fe299bc7209b29" },
                { "en-US", "93ddad594caef19778bdb2cab8957a721e243e5359c24c64ecd7b6ed707204bebca3691f826b4c62c661d580f12ef0db6205d278f439226fdd359b5139912bb0" },
                { "eo", "3e7ac598cdf9df7d4baf79b66080b23403258b84d09e9055788edfe068b402cb7ddab6b631f481d08c7adfe04b48b27a997eddb9570df19323ae1f849bde01d8" },
                { "es-AR", "db4c58bb3d2921e0f7cde71a73ba7f271605fa0e32f22f364a7db872a37a0a09d61257f9516088958be3c0f0058cccbb9d15dd77fe52c2a7e3351189b5797f06" },
                { "es-CL", "f4fd4695c58b7cdd51b8edb81487814d7c175b924c4043efe72def62ebead9463bdf13c56540f7dd595821c5587da143de010076c7d2f90b0dddf73e1bb892db" },
                { "es-ES", "20c8fddb6434a81e8608d93298623b6fe649759de2a06bc16c93809868ae4db16e4768155ddc4015d85351241bb0f8e36d403fe965a8a05d2e97e3aac162ef58" },
                { "es-MX", "83bab9e046e41dc6bf981f9357a1c6bce66e6ab2975c8e8db3a8f925abe3da2b7e44c847b34f12c3175107dfaf611d759d721919fe5389bc2a6f814cb8baa534" },
                { "et", "59b9e332888ab9a1a50c5bf08742def06d89dc2982e30d7fa7d1d56c5138c83af3130e030a01edc3e990b6aa8c6fa55a40b5f1b7216e0f9de7690b639c221cbf" },
                { "eu", "65e9cf92f0c9fefda40f39f07cdd6ca701f9f8092a5e479697a511bf43fcb27471cf5eebaf305a66ea8946bf5c1595481812b876b372853483dfbc4881544f71" },
                { "fa", "ea9df8e08e6071f19721696f756044f75c73f5503c347cb26cd698e384a229012556442938735e116a304ef0fb92aed46f3f78bcece5450dc037a87b28767c34" },
                { "ff", "aff514bc2bc2ba89876297afd9f6f87822832e662fafe3a16329f2b0cef18a06a4bcd3e42bb9467b8b84613135d53a27a0871aa815f159fa5c5d0ea59e7be9c1" },
                { "fi", "ea6ed42bc714fa76770ecd6b489cd3d7b97bcafcec85ee61b590384114aa812eb5af039a777bb2762dc927a7a9e986c10c2b7a46f12c780c7e743402da5403d5" },
                { "fr", "420566d6c3ecf7456b0912d068735824c126d1f0a86fdf173023ad5eb27a355afaefa59a4103b809f3fadf2b56eac9194cd5fdca87b96cefbd8be46e9e8d74a7" },
                { "fy-NL", "d57be52e8f575fe95c0e66e609d37fcf6a92b40290686ef643c9af1da2840469da0c71150c8b4dba652b2a0a812a5b869c40ab9ccda0b6eaa255f522d6fdfa40" },
                { "ga-IE", "f8293c2ffdca72f533404e0f34698d54686a5bfee2a7388018fbfe3b4415e0b6905d1105198ef241df45d36620938a08ffcb147c0074a3635719abbc4acf4012" },
                { "gd", "9367f6437434d1e6f9f762f67aa52e01f123342ff0c08ddd9b476ec8e444871dc5a01ba808a2ce21fd579ca587bdab3124defb92f7f6a439fdabae54eb172717" },
                { "gl", "3851a3383bda6f1390ac208087b1570b00c7caa4820b1f0e77b6ddad978d774c11663088c697d020a6c1c6fc67f8fd82805d7cf71842edf664a1555e6c3ba0e0" },
                { "gn", "2cb8125a660b273824eaee5578bf623f30c0d566ee1883c58c747bc803214194e4ba419a53964cf2961703264cd5494029edf942dc0b2912972b082609a71b5a" },
                { "gu-IN", "a40023f7d80a51e6964a3ce81f34f537e907b8d6bac910e5edaf39af990ff2cd6f2896f187c355fd9c1cec2975be2830c27df76a55ae613e2d5c50923f56376f" },
                { "he", "5f62f95cfc7fc27401758338a7dc1ce19a326c749b35d29f14ff00bcf15ffddc6274d2a2cf1ce9ca2e476bb00f6a4407209224ca567a595371393f9537e5c71c" },
                { "hi-IN", "d3183f439e7d00ea5da3db627942de9eac21a9b6fd2b9f25e76f3130946a6421356c0e69ee7886d3d1df2cf7770a598a15845d6c93e24a7974883e700fecb35c" },
                { "hr", "c5ebc28c267c0ab7e77805fd15dfb159f0e83d5fd419ba0f67d31b808f658b7487d45ebf746f04a4a81f08727419962839e402799dfefcf5bcb12595241a8b17" },
                { "hsb", "fefc3af35c6c2d8bdeae24b8f8847529d81fa8c9d548f5dd91c13245d746fe67ed9dcbc83e25fff633526c0a028b4c137754a7fccd47eaa685f4205e206d9111" },
                { "hu", "75dcf78e56cb735cd8954d71c5c53aa90c50b1ea447b167ff4315ee1bcc8929e0c496d307bc503a8057110f2dc717c90e816c22c31bb4bfed96ab53bf9ffb0d7" },
                { "hy-AM", "6079a9495e3ad187bb841a2eb2cb98bf10cb1b23356fd6029f96c3d508c86525bf8240c8b6cd360409c73e8e41adf0ff1e726ce0a69df07d739a12d121888bb0" },
                { "ia", "65c125c6d8cc376c4078f87582b5fd2d5171410077dd8c556f126ea9cc335990191a282215bb62e041a5a6037816e9a392af11783d8ef44d7ed6088a223f098f" },
                { "id", "75cb4d7831ff76540f78f06d3b8e585404fda41cc4a0b7aefc0ee5199bc25ba891b3657f1ba065416974dde6d37b3d28298201149dc7150c8a965ffe9f0aaf92" },
                { "is", "676e57b0fa39d5d4a78dee5cc8614307dc9bfc87889f693857c8c214664f2a11e850eca66f332ed859cd2e84aef894ef3c4d34126181a53cacd14f9acaaf9bbf" },
                { "it", "7b6ec826b4aef9c4074b25c07815ec94e4447df7debca84d105ad557d5445009f0c5f99a496cf77a5223ebeb17d42307a8d56aecfd2b70363a5e2c6f86deef84" },
                { "ja", "678a7c9e6725b8f83414aace59aa16ce689824d7545bf4518e69d1a54868ce1d18ca504644a6f0ab6776cbdcd27fbbe963d0611f14d514943a8e1df49ec070ba" },
                { "ka", "9118a229972549025ca09ed8ade0ce0ced07d43f917fb3a789700a05537b15892e8c779f4b045b116ffb7385e238e5b1de7fbd470e9fb34721f32bd3909469d3" },
                { "kab", "6cc5c535ab64a70bae4b9d16fa11d7c2d6df5ca924d4cf2faa55eb90083448f7bdc4694df1f0a2c348b183236d7a25945a6b1157e1c45e4370338526e50f02c4" },
                { "kk", "ef0822679b31bb9a6ede014306aef24087517f2ab0be13367d4fb47e26ac7e8da121eee37776435766352a2524fd5f1cce4e2ac524d3d404bdaf686f61add87a" },
                { "km", "4eea54487bf25c35e0ad1e9c45638705f28ee20b4cf6c97c3a53ebc5bb3432240e1437618429ed78f2a7ad6765259d46d404be8bb42f9de8542926ab5ce6d04e" },
                { "kn", "1a8298b7abdfd0d76242c29766c03369fb7e34b90ec38cac9e3854ce0d6019d00e4846c311bc5c2be4f06bedd2ff7ef71685e0e4cf7b0b28976522c69b1586ca" },
                { "ko", "9ebc6363ecd17ba33ee2b160e85e810c434625bf517b20597b724897d46527d0e98a061b0fc0b82270ffb7e032dafcb913b528dffe9a6e4ce921a44f161e3cb4" },
                { "lij", "b20c9a4d82b6c5a52429861ac62d653ae9f07b56d27ae6370514f1782c6d0ad23efb9e102a4974dcf98a4b66314f77fe4e51034bf84857ea1bd4cbe73b022ca5" },
                { "lt", "48bed517756cde7929293826b0ab2396578c1c032f468f606c460c87d9736e8677cb3d471e23098e04bb1f2a43485fe88faf4dda56749bcfc815af1496e7adf8" },
                { "lv", "02c19ecd19b7f5719d79cd9ae0f9abe333a6063bc425c30afd9f4a8de78616df07cd63b80d249339567f919a91709f0d86a765054ce6badf92907c0a94e7bfb8" },
                { "mk", "685b6b47b00c2f69c677bbd87656675c13877c7c3a6f807fa20b08ae331d2c166cdcdf67a2af04a73ea3c9d619b5c2fdf3c4a88bbcd1b77614552aa51e911114" },
                { "mr", "be229d8164bcc6bb12a620a94eb3782494e3dffdc4d0d899977f601897fc8d7eaa07fb811d4d610466a0471ac3e8eb36683231338b3868d65130c05fcec7051a" },
                { "ms", "0ea51544d8a4f79e30fdec1fc5950f219ecbb38a6eb7f6d92a5ea61408cb7af2dacf7044fae5db5458edc4f8b4ec7be6e6f10354a22f56dd3787e7e9c226817f" },
                { "my", "767f52ce5d2341cb6eb7eb8ea3fac2b8ce7cd7f3d4f897b4c517ab34d28d714b182d7e5cf6ca21de0891d12d7f327d25780080a72a366f83b602fad1de11ead7" },
                { "nb-NO", "7ae551578e6ecc32c9833e44b537298e3523500765d48fe6d9b1ca805178813078f8eb4eb905df77dd691bc25339e12d0c988cc8707120192dd3f032dd93a310" },
                { "ne-NP", "172c04eeecfe489719d7b5ccdeea980aef8cf757c4ba24dc9d6bcf5dc871d0eec631d950667cefd46df3f9a4a69303357a3974238231f07f0840cc8792c37421" },
                { "nl", "671f44fd4fe04786f89e5e26e7799cad0c4cbaca8954c479c362eb0cbb0f8835840a270d49edeb16e3a81013b7f652fa834020b0c118add1e9db4c2b868b323c" },
                { "nn-NO", "ae3bdd46d0f94467fbd07cacff2f1acffe034aeee9fc37e7841e0ed4f0b1325e2ce2ff11ee3ff41759870ba74ba59fca55eefbcbac116e434dcd61f106d76478" },
                { "oc", "9171bb759936736ffd21a17cf198bdd679a044a0754bce14fd60c03e79b1db9c790952ac60ef5f3066fe2a0cfecdc8a9b479bed9dd24083729835b025a94f906" },
                { "pa-IN", "c6a227a86b7a587949be3f7014741700420202238f01289a0b2b73153eb19d759335dadd8d67c7b802ed54c24cca9f57ae6ae9a548d3bbe0aed468d7b4f2997e" },
                { "pl", "ac1f467eb641728510bcff1106439d4f3b496b14d664fa949b331b1491e0671abc5fcc4e926d6f4777520b2c2bba46a1061c4a3aa9c3d6064dbe709e211fa46c" },
                { "pt-BR", "4dee41c4c489015554125c5608aaeb7b4816d4272bb39bd163b02d250b0dbd5f94021fd9110e9356053bfca264268facf3d1658df4a70c458280965e86025013" },
                { "pt-PT", "8bbdd08c37113c9f6796fef62672eb8b45c4c5440cee475d93ffc5dfad16186a8214f564ae3b535e21f7f6e547de5f267e9cf7dfa265763b32a4152aea8570a4" },
                { "rm", "f7b503aa65e312f2d97863228799d514e406f34628eb7bea1dd0d476206d876e352a27fe8a487e428e1187de56c7a9ae3ce8e7d4b05aa52a08cd7ec5f0c5189b" },
                { "ro", "0dd92cc8fef17ba8e78b9edcdcbe15003752b296f6886b40608ab79313f612513d8fd78eeae4fdfb318596eb8d2d890d627a7dff47bea763cc680eea57438162" },
                { "ru", "da75b3235035d897a573b6468f9d19929dc298d24efcfdeb741c7cf9c4f924c123889aa43508c00eb0e86aa2d4a68160817db2850d9133ab322f95349bd50bc6" },
                { "sco", "3ad94da5f0713116ddcf2bf6f448a9d5f61ee80b73120f94c4c60db3488999973e6f62202cf82a6bca064b4471fe18ab75beb260961acdb2542585edd4f2455e" },
                { "si", "9329fd3580de721a3384fe5e85b05b57f4677392365784c5c79a5bbca52f602eeda924d6c9eaa7a269b5a88ec6a064109536207cf95a4b1e29f05c78ab99da1b" },
                { "sk", "b0b8c33519065e3ffb5b7e184f400bbece7b6cc30918fbbe06575b69f68fac3f682a989dbdfd1affeb7e9dc8ab550f047c440c2c5444fba020e7cd12465fd944" },
                { "sl", "8725207dbe7753de75271c336175d3c390350d7dd90f1bf527695059a5389bd773c73a7f02ad487a260485a793c2b06229ce3c27201e3b19ae6647eb93288929" },
                { "son", "28b5abb59c00891634f210764cc5e42018f20e9ac49c3de3999df7f7ab463b348e1529518378aa46aabfef59d405eb39f45a8d9bb9753e2af7757c1e557002c8" },
                { "sq", "8f0fa4a0f7ea8554e45be2fcc3ddac0aa128d725d67daf717331e1b493f901a5a0f1d289c0e87cf950f957e00d1f13cf1fac9669a1000ea92762f47c5792c168" },
                { "sr", "8dcc5994147e6ad83067873b45c4f5810023e653e42335d46c9ed0afeb9124412b010d84a3859fae1145204537b4b8f36bacae2b1bfd739b2360820a660a957c" },
                { "sv-SE", "f4c4a68170e0ed52fa4be2594aae5659eac2c6b0d693735317e65ccb69efaf392c9d0f3b491593466f1d659ff2120147b5c74b2b0072c905332f40b3d19c6faa" },
                { "szl", "845fbeef0fb1f92917940502300cc82253a29a40ad1ef4840dc71f9a66832f1943b36c171d00a22e8de4c402d62279083f54ef34a05c2381773a9d2e9edad772" },
                { "ta", "430b1b8d52cc3b0621ff4ee04ce69fe7a56adfce1fde28211255e97b9199817eedf4a1f232811d9f91cb7d09a3f1088035d35b55970b7ba9e8789f3e49731b2b" },
                { "te", "4e04410efbb0fafd7ca33fd9e132fcda9679372950691b3fbbd6650594b6506fdb45adfe3e43608f3d2aa8afc0f7ffc844a5a0af10783b44456ac03296697a47" },
                { "th", "83c084ba988ab1c80c608fe4ed165bf4208006fb5bd3d6bf73b8e5cb24552fc516d98fdfb215172349ed0a2f5590fd6938e9aa6e78d39b0759fdcfa85dffb95f" },
                { "tl", "f9c7795daf2c596002b46ea32d8e65bd9d2551917a86bd2893697cf29e0e5b9a163f5f55bb79b66d2e8eac8d8105ce7dccebb2927ac86be1a1b4630061aebbc2" },
                { "tr", "cc490b799125fe0de1a73417b5750777899d15b69e678a3b00b9cf8ce562e0a6e9b015f242589545ff30e85dd288732b3ec18ba92b2ef361e0d1ee498033db1b" },
                { "trs", "2981832e3959efe3de07b11c107be707fc3a32a33899a6ebd030cc5ecd86bf1a5bd278143dca5fb0f49a72dc4ad0f11a37758e08d511788524713afe00324ba2" },
                { "uk", "0a21ca71abdbf8caea321eb029323709a92a26bd41e2ce8a5a12e7382b4ef2f237d14d586ee7271cae2e7a7e5b529c171f990ce29b141193660d3428be4f5e28" },
                { "ur", "86fa12c40bdac079efd4d600abcd7b9caab0c4eb8485488cedf9e7131ec0899786b10de5e3432ab971957549f14c6364353ffe0b9da4a49644af7f47183e0a84" },
                { "uz", "88ac6ea68961534fe0f09abdd1e479f7c76c2917492ba6b28c65308144a1befbc6c3566e295b076f856dc6c8aaa40502f1b55ff32321ba13b061eb8c05c5c2da" },
                { "vi", "672791c980b04fa2fd1887cde51aa0550a79c3804b686f2a302e92d2c68b843cf3c304e9aa5d0969e18cd0aa57085e11a9a0c52dbf0c38fd53f754894925057d" },
                { "xh", "0cf18ad0a3cd97bee70d0b5d4806c5d899352a35f920b96bce7deb2f5b910cc7e55c5ca533203c76e9bbdee7afc4ccf753ecb6a49656c31be99d0d2132ee35b4" },
                { "zh-CN", "0f03d26918d70a8a66ae081140c3556e9ae7b378cdb7f63da54ff0b200e3034ce8e23247dac30d6244c3a0cc193e6e6b5c291f44a445601dccb4d5a4f0382126" },
                { "zh-TW", "a156445478ca8813d0dd1ae2ab673aeb166575aceec39b5e63423d5201db63680fc1655a738bf51a051de4fb6f6f9b235c32fe986576a10a38353264c0a01715" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/104.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ef140e26df640b4c2b480c655b45b0f772c793cb95faf447002769921431a0388215d82be916fd1c656988352c46ab9131e968e2fabf246bf799703189f48fdf" },
                { "af", "e219c048715604da2a0c2b0bfc14edd506df9b2a43e9331b6be195d0187deeb5162fe1ff23a818098691f3c1d7ce84f4f6fc2f67ced37064e05c8c781bd74c93" },
                { "an", "51f35b3978c05b7bb3454fd839c04717f775f8647dde03655fcab9326458dea60813f95709f15d52dcd8f0bf88f5ee7659328fb99919e8b635eb47ef822d9376" },
                { "ar", "aabeb6fb567a192edace666a9b32fb0f1982b53514ec806dc2cb452c5fd6b710f944a31dcfe06b44de253fa2080f5364b946a26ebfd35a5fd41c5912da5cf48c" },
                { "ast", "759b2a3a2b63fd0f9be294bf53258b55d9b5827b8938315caefe8c6e9d8b450a155c9d145e9f43ded5719e99364f746f539d1b48f18adce045ac009520783bd4" },
                { "az", "a340edaa966a767f6ac8ac615fbdd8649ef29ed57292474f724e96edb540562806b5b76367c79dfb4c5eb9c4b27a194677099c4a49008c563a060a3bad65f0ed" },
                { "be", "b86a66df2c5d8d3f115737ef2b1069d39c21b51c02eb35b050fabe19ca4f0092d9a456bd7f240780229a0f1b981a70dcf5c5f33147789dfc211955468a7bb42a" },
                { "bg", "aa79d030a154b1bf343a628e6368d0f15a03158656bc18887a10a04d1dd095a68afb71d321f88a1477c65a30fd1b920ebdaa481e7c7f70e45203a92fa7340e29" },
                { "bn", "2fcd2d6d0ae2970830a2260fe140a836cca2a866997045f804894cac843a67fa74dd1c2535605fb5048d6b9d8fbf2114156b8d19938e2f7dd2d7f26aa748c946" },
                { "br", "4633a500681d49044f5a2676a7bd106f8ee78f8ce1e4dec7e675436aa2602d7ece031d64b75095b86887928e90ba88c41fcb5daeec85db5d9e3b879358a4989c" },
                { "bs", "2edb6d9a492c2fce9b02591e8db16e8eb7102f04555c6a231206cce3efe4a73f2b4e92c2f6b9d5a2dd8b18ba349a695e0745fd70268c41c92a54d02d3839143c" },
                { "ca", "8dc22bc2a55ff2b7e9dd2c509d239ffab3cc182152260bcf8dbda25658fc3915fe6623a01d0fe00372ba2f286e59b435543beb9e8e460496d341319865bcf313" },
                { "cak", "dbf5c5365dd7a38cd54ddb15da4d8ab3e8fe39802fac7f6be18d66bdef3b171e1193a60e85a7783814af672fed60f7a928b2bbe6be80440e1629408d003c9e98" },
                { "cs", "4dc51fd053821bfde7f431348adc03dc372cb4183f8c61cc23f5c68005c9c761cf81f110ca98b57cf08c64be509416510242ccb7a0fec9cff2f99f26006dca89" },
                { "cy", "f1f233e2e3e5c6a725f46f99974a2ebdda3b390364c2cac282773b893beb9219e85f43ca87c47d0fe040ce08f3fe7c4759c86769e4886513bab54c407282e7f3" },
                { "da", "f9b3c2ed2d82de2f291163e9d70174b1a393c6c7274a5d425d4161b29803e5f560b7df63dc1f62bd89ea54586c458dcce0de4abdb8b17c5dbc111c01ea44493f" },
                { "de", "ca3075c2afe73f7f8463c8b7366f482d634478e59dda6982472aae9ad230769e8a718c85cd2fcabf6f6433a4d9d49a163ffd0ca63e1221f287247cbd9e32dd39" },
                { "dsb", "612007ae461613476a322ce5af6048a9aeaf1b329992bfeb70035a8676700968eeb7b62c294bc6a082b58d00711a58243049f3112d98bb3e4e559bb74390a303" },
                { "el", "81b601cc3b176fcd568ce2dda8932a304dd3c05f1edae7cf56dc98ca7de0eb4bd2517d7dc44d58e55cff870b3c7d3312de084f4c30002ca53a16ddb7c4725362" },
                { "en-CA", "68790e1ab1e50997eea77840e44c446cb25bd5acae3c271919a11f6721667ce585a9678e5fa9d83b4cdd6de862a7e4c3ec8787be81e94ca47e09a09a18fa1dad" },
                { "en-GB", "62a2b2b2ccdc41c296c5a8917f45026b501b68ff7b08004699219f30195cd2f59f031df3bb4e9dc7edf9b3418129842541826e3ef507e8d3306524352610eac5" },
                { "en-US", "a236d37b358ae224926951dcd9a43677d52edcc41f88df642ccac04a7c92182dd629652a25a6cb0ab092131df1ca76763011197d36520b667b88517a3432188a" },
                { "eo", "95f421c421bcd3c1e3d622dbc4cee020a8537c120c54707d9bb454a62fb4e835792d8aaeccce053469b2e1a6baa2130f1fc27c1295fe9c7491764fca4955ccbc" },
                { "es-AR", "30ec0af321adacd048614a2b97c0c00f5986bb4ddc664130b7b92a84b20f3d251f2da67e0a6ed1f34877fea61155b9dfbbf88ba9a281a7e7ff43497721ab8425" },
                { "es-CL", "2b61c3cf5398562f45a920d2a5a408450386559a2319fc681c17efe98973a3c3a1e31a83c28ff54f9d248470b76a5671b333719c82ac31b359f48bbb9dcadf31" },
                { "es-ES", "83dba522a9682a94c90bf9a424aa5502518e47abede3204bff8e173c309b989bac790f84f54ac62f249356ee3fa2b6526703b86e598934417f635d9faa3b4205" },
                { "es-MX", "458b5b9bf7cc0d64967f4fd5491a8b1bc4756306e6838d248ce8791513ba28e38212ba772ef0a92202af137a2e5f96feb8c2583b585813a3ced95beb8b284284" },
                { "et", "bb9c6dfbe7a8374b21893198c185227c09d25109fb5557175f33130982d076c9bf2295eeb9ef542bd226f5c7809163aff6a21a5a17971b29d41e77b7d0db1974" },
                { "eu", "523235cf03a9ca09b90b915ececbbda69c1f48cf1de083636b054cf826cd5c172f464867cf122701639145178f1630ccbb13d5071abbb0f1451ea3c725c90f25" },
                { "fa", "b319dd48b02250b67b26b4830e622966b44e4ca869ee0438b06ef5f00959fe9260eb9131c7a5d63bc7184ffc1de17f955610ee7957bd52a86d8a97018e288915" },
                { "ff", "eb2cdb97df5607422ee4d1c9e188174b238026955108c5bff7bc27041e9e2a8bbe83d80b40e08aabc3a355dd35b5b7906bdb9974ea378baa746121dc61c6eac0" },
                { "fi", "2d2ea427cb47091256a6719bbe87a7c42e695ffea0cd74bb44fe20938fbc391233df4e65251ea0f09277e4ac7748596a1454a4846c11aeaa0f3ddc98d1dbdd0a" },
                { "fr", "7062e89e7bd4b8da743be446e463e7eb9add7ce120bfa57aafd1b94f0a34072b7cc277479aa498e8a1146e461855040eb14ac6d3814ad8045e6ac4a18e262ec1" },
                { "fy-NL", "88a9685b80bff808c09f2039ca6798b96d8bc61d07140b7882e6a577bafe072fd8b9b1063b911892376580a62ea05a88cc1a1c8d9bdfe8be651db079ec8a4374" },
                { "ga-IE", "88f52c11aff47f8c1d19e5ba3b2c95db0c3fa98a64b6324435db1e7bf779ba1b66192294723c98190cd5c49f170ce2a5bb70c6d3ba0e21aa476885dbb305fe98" },
                { "gd", "3a7208e6fd57209f9de3acb612a9c316b6c2e2aa2023377851e8cd02d4e34f8323b93c5ea4dcfeabd971f874d332f5ab7c8eacdf489e42a4a7d5628b70ab8ba7" },
                { "gl", "3cb127e0a16648f0ddb6c379bbf1bb3c6bc0a9e5da7ece046396ed8014bd55e6ab7bb3226aecba916cad14a7e79de6b03d4d76e71770b12359838a7f6a1d75f9" },
                { "gn", "4a62c39ded30346157010d13332288d67f24e04eae3f5646fcfb22901f94ce786f4f4086d4988e91ae62fce4563cabaeb30664e4a9b8f0243ed91618790bc7b5" },
                { "gu-IN", "f593777a70257b6f77099ef34d0da638f14f032dca404d2ab33410b0b6fdcb18e6d0fd794417fa15f20684ced570844019db6eabfd1f4a1c5a553b8a0f72be5b" },
                { "he", "60f61f2d9c88ca655c08f96eadd60ee1be0dfd74d89dfc00f2e75cda9480e01c5985d47d7a2f17bc1c066d9a597c75487c333d170272d674e2a79387c09b92c6" },
                { "hi-IN", "d03064dd92cd5b05e79675e25a238563d96111035f94e48cbd2cc54c92209493ad8f5fadce0939013079c79344f2c03a8d709e5e9dcd19287dbed20db5ab573f" },
                { "hr", "12b31598ee928fd6c72c9a60d1e69abdc4ebd241728b1fd50f5db0c0a38b26414683bb0d314028c519c085591486f7062e2a9cd595f0c05d173c112856eac853" },
                { "hsb", "401351bb93112d597760c3773948c819feb4e07f68951d5bec498a3674c27dddb67a95392001e4005d4b54dc419570f3ddd93b1c164ee16c2ec0b59d80e0d972" },
                { "hu", "d0d7cb212c42122ff02d22e86f301ab2570a856481fdee83db1e8132db85a6166adba00489e4e6023f5d35a8e7b6fcd26d8417ee7716f2a60d6bcfee3ba0ef19" },
                { "hy-AM", "ca3015eb580fdc3e6567228425eaeae104398bae83748860af49b65a1d0e2d80abe9ca53a5ab0bbdfa21af9cef150421cac1958c7c23d522daa7234ce34877f0" },
                { "ia", "9a7093dfeb4c8a82fddf843cc6a42c7817b9e4195b8cc24018b697df3c086931b261421b2bb4f516139dddfb210b83d28f014df1f26bc35c0dd22c175f3bd1a2" },
                { "id", "86b87e1181a455a50fd06dc9712a51f9ebbd95f21d744c22f87bcbdb56bbafba7f6af4f70acc498d978ac2adf11fe08edd224d45d1bc5b54968f3fd1b29fe3e7" },
                { "is", "2c4a7c4004558912c882827a7f13570bf1e05b2e8553fcbc2413b2bdd2590190d0ccc073b1bee9b93eaa1315c77067091c78429bc8b0104d401d0d1ea35df02d" },
                { "it", "e9ec6b1c33359f2166075bb3622eaf26b2ff72922ff66bbad544e0b3528bd6cdd0ba3c91dab75cedda82e2aa1be06338d812e527829f21f5fd308b3819dc2392" },
                { "ja", "0f1bf91080c844fb683989391a6f43b48342660e3c96446f8c6c4a6e443a9128d2f0e7a89688fe8113c633e538525f83c8c98096bf0974ac9d85b56912f61e9f" },
                { "ka", "4926dba0c241fc0f31ccf688bbb2f0fa3aa7fbb324229d1d540bca346e80cb44951e390ebec7063dab069e030ab567e13c8235ca3490c1068b2692260b1548f7" },
                { "kab", "5e824056974192e48dc19e74aa586f82e6a43244e368accbcafeea2a5c2fb08606d01a325e01c0bde79e4341d73a2b12292cdcb4d4a9fa35ca3d7df57eaa7b35" },
                { "kk", "fb35eeeaa833bc8c29d8054093d545598c1dfed8b676bb01c98b6959a15e84276fac9bc3aa8848c93aec9d8b260c87b21a12663ad3775e18e35492323417fdcb" },
                { "km", "9e3d86b49d37f51dcb7ac8ab9cbd8a0ce4b864c9a0a08f8d878b63cdab99d25a52c91b3307a7445687e6bbf7a415eef24d05b181057fd803ed51ee5201d35739" },
                { "kn", "b833f85a49d6e644cd945dc194dc38e80d83876b8c7c14f5db570ea95e4038611453dca6d5cd2c7649401e13fcbd06fc484e25e2f9b75617297b9dad15a35ebf" },
                { "ko", "949c555f13e0f198dae9761a5014612e11cce217f4443f9f3782623c19a9dbad966476ecf506cf994f7f94c6579a8661cb95a60bde531e442b9f8a5df37e4369" },
                { "lij", "3f68d557318d634b6ceb957240b534be379ea4ef2d1dc5e3e38670982ba5839037b8d3f05f7302729c89de83b3d34dfb04b8869c0df808f592c865a35c51e000" },
                { "lt", "b4dd927bbf88f8ed81403ff3dc7b7b54738b03564f0cd7316baf519fb0f419f6008155181d93ca113376d90e63214c6bd736d96e15eea1c208e5e5acc2356f42" },
                { "lv", "923857058fcbc7cc312491daaa6114ed6a8bb89f5fb839714b662cfe3a17252b8202c3f31c1149303d9f85c76874e4a8738adaec59973937576797b450c56d78" },
                { "mk", "c5dc5a595619764dbee425574754a58c3ce2ffc0da231ae83b16bbcf86b2db33ebb34d99ab36acf0571886714f7288b6952b2f3093c0ec5a5cc53552f189ed97" },
                { "mr", "585dc5093fe6446ccc1a71ddde640e8ca740919b0408c436934a9de40c1bf576dfa449f11cf144446b775825ae37a598bcf11ef63c6bf983e0b7051f82f45f67" },
                { "ms", "99b40c9d21a255520ebd946d5436e076b7f3b1acf3a78d39311273547a212528ef7ded5b0eeb939c23f617676fd26e6c25bfa79421da6ff293aa48903c7fe324" },
                { "my", "d05f2f77798aa17f28dc58c87152884ac865f32d128163225d447a09c6348246f3f0519c8e2614301525f4087dc8c4143612497e9f2173230199272a8594a7fc" },
                { "nb-NO", "db85888765f1ea8c830a7cb86a0d479533175c8bf2864f75eeaff2b75ed5daa771f8fa8563be9c759713c5f82a88c75788c412e318175d1ed344aff45b77a031" },
                { "ne-NP", "3722e93e798d475e72163fea9122650a6dbf1e191f76fddef90814f9fca61a7f376f7ee30655ef934bf8528a35445f74b04115d40246b1f246ce4873b7d6ce7f" },
                { "nl", "c3c5de8539653eb814681e36bf65f2d2a118ef071aeec80bfd25928f2fa5dbe1b438d2bfb9f98a20341da2a067d61838267f785ae5e546da597d3bc4e129611d" },
                { "nn-NO", "c60cfec6884d59b5da04dedeb4ce5e0ec9f06decc9687770376209955e16abaafcad6287f0f458ab36bd20030f3e48c9287dcb377c1322803c406fac6659c76b" },
                { "oc", "6fb529a5f9042f300855ec61f17bf2799350308fac70573a88f34a13c6baa411061ec2de2dcf94d59843c37bffa94e5e152328302b1404d94ca21fd3ba0ff435" },
                { "pa-IN", "a0f07b26beef903f8d57e9986a868d5da7cacb9d339edee43f90ef322f48ea11cc08970339361cb2e61ec7b89f646985481146ac179b392f50a9aacaea1366c4" },
                { "pl", "f081d2685f17d90a4812298faf2c5d371485587baea6657366126cc533891d94faa94c67a272c1609f840b71092430e861af815a8565f75a1309fdfb041020f7" },
                { "pt-BR", "cb092c4be404f42557c0a953c0362b1e5e1a0dff471145f82ba62cf5aca7afbc318a772256455bf570c563b605a904bcdb6d8c8ce4efb2325c7b153c7bb5619f" },
                { "pt-PT", "9491e742d70b6fb66f3cf942e0005676bf3b953e70780bf313cda9faa6345f968d1641e449d56ece4d390595707d6ace22e6e23033b1360fa67f8e0c4ee13a46" },
                { "rm", "671aaeb01f94501b043172a456f45e7a573de889dcfb817a1443e9e942850cdf9aca9d7b5d260b74c5e15279f560f7e5c869b384f0df0e589b04ed6270d8c0f6" },
                { "ro", "6c80bd0ffccc8eb1a2f573c8a7a1f9a5d6d443a7a62c1b7e0049f121ca8ca8b321dd1b2c270c6812420cf12e5b209fcdf5054c0cf7d6ffb50a941f346170003d" },
                { "ru", "23cce664ae4a15b5b5292633de3b6cc233ccc47e86dbf66ddc539e22c3f5e479e29225d32ddf6263f37994abe88cde8d766267fdb30914b78fb467a1afe32f61" },
                { "sco", "2958544f5474588b006c966f890c39d6710929dc438a181a0d508187b421487f8b62347230f295f4bda8af93e95ca81a8b7f30e7d05c3658f297da4e69b0221e" },
                { "si", "b0f08ae722b752d8a61e2e409f3b7ff05f68fdb50d5b478fc06623b555275bd30e9f5066ec4708d5a4d2c47674305786f89a17453984c6e6ccba2dbd4fb59df2" },
                { "sk", "4924f9400fb6e11ae0d7404b927a795774f23910eaec91522530a9a41967e5e9bfa912240a79e38133b656568bbec34e7d2a12bfb5f1c7240abc5810e913c4e2" },
                { "sl", "4d017d32930f446b6c9cb2593f642651f70754efcf7dcea3c7074f2eb8b822ae5623783e69e50f160fd7dec290a2175d309bbe5ac3ea4d1a064ff7d50629ec82" },
                { "son", "cd5a0fdfb3e56d09ee76c0b95794cfe9e3e423006d13a006d94b08475f1ab353ee8eff621f0fcf651052c56a67df4e148ba596d40b41c1e60e38f055f27e158d" },
                { "sq", "596d540567b53046d82036a4aa73c3289b1211f07f9c1b7924a4f5a7255f113f7594b3409848a672db743665e17017aba811f7dcb29a17520d0244a379d71e87" },
                { "sr", "89bd8a4bed332d388ea30d9927bf57a999a647f33aef2f4086b0cb2e4d0ad0925946a050a30d6eb0b1756a2f6365db8e505cda149ebc560a7ca15c8d13a449eb" },
                { "sv-SE", "db7a758d881673d8368ec4484c4cda3707dc5742b5b56c5e2b73c8bf0fa47a3c148afcc596369baceb0c5f82ba73c366c79eb14cb7cd68504098be36ff65b8c6" },
                { "szl", "f7860fc1cd879c126301cf1f95c53a7bc7eed2b9b1e421523a9ba8ab306ba73454748e9a723f44d65c3afce1decfb3619e2c72379017e4a24fab3e0279e1f9bd" },
                { "ta", "9ffcc47058057ef747933d8cc65e9ea39bacd0f24eb08bc63c17b5ea522e3b77d76910b94bda78ad6a2201d38c59ec6d5ff952c99973c05fa46ded1f4c9a35fa" },
                { "te", "9236e4dac8a8c15394c79bab09ebc3ed0f9880dd1ab988508808ae34e5320f6ba267a07cdd2bcadba699aed0d813e04f480baf9a134977aa8e7d57746927c465" },
                { "th", "8ff38c196e3ac177771432e3c924eeb7ad451a6839b185a74d490c8489d694e592e996e14eab67cd7f7ff48faca7d8d819e9e71e976e16bc33c48a748ac0e61d" },
                { "tl", "b1f36f5e8a41c1d2fa057520f2b34e7f3ffab20227fb7e2a1d8b73d9ce240bc188d34975322da902de102dfabf1f95e26a16e772d9d6bc2d40e5db056109c6f1" },
                { "tr", "38e4797b68deb07c26aa2124d341261750871a00aed05fb602d6e3f11ffdb9a8a9a6ad27b5ad3ad0404be23621a8529a70646954400025d19fd58565acb24fee" },
                { "trs", "ccc28e0bbed7977e84f6ea166e1d16615a66b0f6fff7e653d4e8b8f0ea57d33ae2229eb779349cbc5b694fc9b57de733fef9598b6a6b6f3b066d8b3ac92a0120" },
                { "uk", "ca51aec65f099bc0b317621027df55dc3bd4c86ed3467196f0076d5c27ac43d5952c35e6a8a4fbff69b15fa95d567208df850fab281b373721cecb6081501894" },
                { "ur", "33863960af7df510734a1adb17d8520c3f24b379a75faff1c5aaf2915e4c50cb0a07efbc73a1556ff3cecc939db28d1d1fb410a37a29a82a1a1e44673c7e8461" },
                { "uz", "9c034eafc7c00716565b38259453963eaf4e2965136575a74efbbef1b93d5d505095a04fbf59b38c6e119b6f8fab8550af350d3e22252311cb811c3034441787" },
                { "vi", "81592dfea0b13dda705a4f00d9906f6e35ae50fb3ca8eda186872e4493db2095e765cd0f0f55b215f76aa7da793b7e652ca32cfb20780522dcca7252ca0db594" },
                { "xh", "e03d3954cff8642128a1969e1bba89ea993a510bed4baedecedc9ea2e715028a46c646833cae0d5228a25874a47f9c73f91ef73d4e7d914bac40b1a28421f0f6" },
                { "zh-CN", "4ffd8482bc327bc6617da2ef843568ecc6d9502105f0f443c226632567c0779691daea95810e14e083461e054ce35f720d95b0ddf9dc460531eeb6793e24cef9" },
                { "zh-TW", "1644661412bdbfc28493f4932b2f6e86544d5952492f241e534e3d4713e35defa91ade46313ba4675eecf10871a9c00d30188d8c1cc6c0eaa9d8412b52a77ed4" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "104.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
