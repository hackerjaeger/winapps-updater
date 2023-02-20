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
        private const string currentVersion = "111.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b3/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "cbb5632081e0f996bf870153ffbf5dfb051718f0c64c89e45c3f4c14d03acef51f625ba606d736a6fcec03682cea58ed40a3fc26765657bc933e5c4492c22118" },
                { "af", "90e8aa1942fa9b15d141a7945f8df4ab5adb7de3ab097953316e68e75f7ef0af79cce8ca7c68aed36fac5b2fbe3dc04d54c99e5d48396882032d7eb168f5b3cb" },
                { "an", "2e8df789eb137ddf79cc483cd49fe5b883f329011ec753b553ef50c56c97dd6fbcd8ed7a771dd8d36772d3a4e9d8b2e9844d575495a50a0cde0246285c7a3cfc" },
                { "ar", "66a0f3cb76fbd6df9e5921e112533e53fcca0966c31140788a9696d633f3b5967c6c5248b0104657d6e72261754d8d8908c9629a5bd4f4581c0a8f546d47cf12" },
                { "ast", "4d75c6066c9c3c3e795efb7483018a2fe45d2eb16f76a65cc1ec1a54d208507fcf5a4834d5a2f9c5d9194e0b1eb26c860cc36f3739927a12113c79629f0872e5" },
                { "az", "9569a1bba29f10918a9de375a59e4420219b23e678ec1460ef522751663479139fa05d63428751cf1b1c42172f4bbe94111f03251eee43473ac6d91a792d20f7" },
                { "be", "6bc7f14776ffceb447d8a847cd0077d58e93d692e62bfc7e074b09288c20ff6463964b08f667a6f47f2c4b80a16d9b1bf1a0eb9793d54cfd2f526a720cbd2a26" },
                { "bg", "b446d6e098f7737915ae5790e59a4f255f402387e4e94c90a9fa79e1ac42b7e4dc71aca2d2e7ec78fc057357d9d840ad956c6bd0069ee91c1e1fa4d85708db33" },
                { "bn", "b133f35ddd7780a28e173ac8f2c4f7a9af3a017d54fba5080471658cca0b0a32e6efc87625e38cf5144a5a3774577d5691c6fd31ca84aba28e0f4c0587dcab17" },
                { "br", "a9dc181d3d0e76a2aa0df78cbdfd7c12a7f7cde73ecddd76a1032c4875ae97543abd3a9e657fcadca215b2ebc05c4f1d14375d664b45e3395c3ee1d1cf7a6793" },
                { "bs", "cb77a87a189fe69823c9711a5e056a701dbb102e8b4aca8cae0cba64a15d00310525df0e9e34598aa37d8b13fe7c3721744d688ca59fc9465319c5535e681cb8" },
                { "ca", "262953f1816792f5bbf49dc260c0604e05c71776d3f6bd9eb2c97fb54a1ba01c55b47ad33d7fe20d8f6f90730699a8d980b679ead50262f37926775396cfb119" },
                { "cak", "379cab9a9869c2cd4c7925a0d31f404a6ca9e9ba1998b60949680825a6ae3ddd80aa68e19689391fdf746697b0e1025097f797f892f977d7f4faae21d8fb5cd5" },
                { "cs", "2de96ffc23d06f377ea015cb8ba5e5280ebd18a8c9c26c5f345492a60e1f98780471b0bce950ad16def18988fcccaa06ebcd769877ce32ae5f5cded98ec934a7" },
                { "cy", "c3e3e01799fa6aa8ea003eecf600b079affa5522ef62f3f4c5c38845dd9b1d10f8ef7342f2b1dc583402235da01e39bdce24a7e57065aa90f241ad8e8d44362d" },
                { "da", "c3ce140fda591b00e3b1e8a41151374b654de152980b7bb60dd5eb46df49e873ffc14deb33b1a29fc1c1eb47090ebb453fb9f6c004fc83bd7146498bbf941748" },
                { "de", "bbd8e3a25831f6cc641d91b7d83a406483bfdbb0b4055e0b777f3812515d64838de9a6f6639351eb6cd16b80e2742a020d02f01471a66a0ef69f218d6a37960f" },
                { "dsb", "a6560223ff544201a1259b71dab9d6ff021b94bd2313d9b2b46198fc870e650342438e2141c53ff66be6e9d3a8aaa4bbf6ddeefbd59c47957c7133522b8b0077" },
                { "el", "78a19ebf26104c2e8ab03f28a18ad2d21aeb7ca0af54b29259f3535d0929cd0f4717dd0a94200def2c3645adac0ac1a53cb119d76bab434bf125f72e9a2d8c59" },
                { "en-CA", "a8f3b7b7476e7ee1eb8d78e16c4ac83caaa64940a74473ed6d2d65b599424f55184d52c224cbb54879d5bb6753a7e97f4a02334158af59f6e5b071a459bea1b2" },
                { "en-GB", "9bbc5c4d3f369d2e7c3e6fd95acca714598b05d5d48976e1d797cdfb762b83f062851612799baba670f22aa6208ca1f1ac18da1a98c2212bcc3c635b9cc488f8" },
                { "en-US", "264c67dd29a501f7a599d597a10b43de25ad70157fe0fc28e5f62d5a50f9b184dccca402a50d3c8f28fdd6adf6c2a14b38b24282440d7b6043c94afead35776d" },
                { "eo", "5ffd26875824a007e0e90998f592d31d0a0275c6aaf6ac25b1454f5ad221ab369fe9f636d8dd52f8519b004ed6923c79a24586a60d9c0fd2162ca2109db417f8" },
                { "es-AR", "11fd76aeda2670e6fd2b88a2e6c9cbe39098747edcd31706138585270283e6629fcbce00795ea4598b9ac04f327a0c87ba14f0ccf2989da8d43ff4c630ecd71c" },
                { "es-CL", "2517c5cca9db0e736707c68b6472dd0a36ce6098e63f08e6869847ad65464c04ccc411875411348e091b0b2e005376b6a427a7515d9004ba59c1553d9104826e" },
                { "es-ES", "0175f3ff5b637c3d637c452278f8e70fb46b3c31b02df6077d9ae9db77b8ca981c9cfc1315a38864cd1f6abe219e95fb9c8febafc14048d13b5f28c8c2aa7651" },
                { "es-MX", "27b82c91bbd8662056f59492ce30b4dcda925ee7464aa16c3abd4ca49c16d32ca0cf9a8ae72e0fcb9c6da6a5124a3a5588ac0db4192f4b80b2fedc9f73e8e783" },
                { "et", "7668bb69fe5531ecfdf1739f692e6d740e2066f734788e4bb9b5411ec1b979b22218b7bfb83292fa327941ae859b5d131a78416f70feb66de6af7e09c8e62d88" },
                { "eu", "40f6be89c1b046040d4c24ae8f92ade808bd535391c49891d490dc0ef8f92491b151f1daad3aa09049da3adb91f4842a1f6e4fcd55bb2a84c6d16ea83b0953dc" },
                { "fa", "085314fe981720501be6fd15ebfd6fb55bcb1f57e0c492ccc583d2d7ce154d19bd23d7e158da49f9de354e53a9e0343f454de20370b4a174cd8a16638f6b78ce" },
                { "ff", "dfb2cdc37f51a6fb2e82cda3aded3bb42988bb26939e2ad2fbe67d618cf7667d30fde5718043e480b31746dae52f577bdee887b5ad8f9c8bc8809dc2e3f06d97" },
                { "fi", "6697d8100caead18155ffeff084f481a03f4f9097c2f7b3fb13d85c3e6389f93f7044e7913da54574f3a9b9232d7580313ea7152cc9ba3f7083fca4dad19607b" },
                { "fr", "f22eeada2e5732ff3e962ed55be8e9dacd6c6a64e2698df47be16fe8f800b07762af217f91dc470ecf768c21c52e0e0f061060d3cea8607c9dbe62c0dd6bf008" },
                { "fur", "c09e1bb38766121ab6051c548c39c6de023c5bf211e68182937e864119dfe99e0b91238b59abab52a154aa148a1586ecb8d3503cb5ebfd77b7a400f27b92a368" },
                { "fy-NL", "4e1c08f3b44afb52a4212196a2c7a8eaa125711ec070de37b8b282739f2e91abaef0acda7315385bee2a745cea7950a47011001eb5a71c3976127610ac1c5437" },
                { "ga-IE", "10525e6ec16792eb8d25d11b87acfe299ddc3bbb064cfadc2cd2c1b6e858109a7975536a15b93167fd5599a25d8b54a9d5da62b6a03119a8d7fe8821eebd9186" },
                { "gd", "8258b39b59faba8d7d511c3313f7e595115c023869c23100959a76518e799d3e017177de69a7c88375cb12dca0c89d7878bc0cead11f925b81df0b7255fc5f3e" },
                { "gl", "63c56594276c461613070e81195e93c43bda7f43917973183fff2f3bb0ed670b1e67c59ff55d20c822d68cfbe2337e0461f47ebee370794eb8eef18de33a398a" },
                { "gn", "d5d6239e31a5da92e2f4d849139547c9366cba9902cff6a48fcf32b7aed14001a169440a0272d328af12f3805769f8bdb21bf0ea220703fa40796a958c0a920e" },
                { "gu-IN", "93c32dd3736bdfcca93f3b8dac60066659444f574d86e2fa8ab5241ee657d6e3d8d849554cc47b834ed3e1444a94aa1c1c769d6edf4ffbedfde596afa0dfc62c" },
                { "he", "3dac7bf43e2b05cf37b8090ce2efd51153ecce9dd447072b1daed2ed501a8d540a5e146c00e4c1ebeb2d0ddca119647b6417e8d143b13292c4f2b4742b02cf1f" },
                { "hi-IN", "e1bddb6cfc84e645ac58acfd21dac6b1d4e9c88f6982f24703967122c21b2281b18f00d1201f14af7a2f79482bfa3988ded72461cdd3373ac9ca2ae7f336c63b" },
                { "hr", "453f357236dd307e2df5dadf24100f218745d6b45f5df446f0d166e6abbf259be7f0fee0877882381db90d33b78f68449ae698b1dcedd3a572c9d404f1677f67" },
                { "hsb", "bb17fcb42e3b95aae11314d46b06cda4c8a3a284934abfa982339d5ffa163d0f5fbc7cb117159fea984033574baf9814af83b74396176d969c4d268a49dbac0b" },
                { "hu", "cb58f70644fd593de117d7d9af7ee203373aa683294597f58ba0cd08e5513acfc99c65027f5e657f12d43acdc30872325c62617b81184bddae291f0339cf240a" },
                { "hy-AM", "61f97520bd8a0869f70fe48b59a06dd726e6b0cc2a85c70d9eb3faccdc64eb538d93a9b927d9456afb588f58f767fa9a7a25ca44fc8f9ac2a607cc0a37edc508" },
                { "ia", "09298fc07d38fc3342df9ba5d1dc7cc5fa0688c7df6379f236a7d8a8d17798bee4cfec50cb0c6c32428422b94c0a9e6c3144ea3d277120af1d20adc69cc9dfcd" },
                { "id", "0b4b03c493b56c25a4a7694df47808faf8ddf89b985727d46df7eba1d9a2b3bc4ef22115998fb40016956a2eee65c8402c00a69857c9731491b0e825c3476299" },
                { "is", "fc79c44c31ac7334f249fc826dc642c02b118fb7780f44d358a82a17798925214a58d129a2a7dca2a443f4c6c0d20dc6e671ee2c37226d3c5b1b5798d1dc4819" },
                { "it", "113eb607ab36213a182ad7c41c74abb4b8fd2fe96eee36c96f8b3eacb4a0c72832adf50b341348b83b5ca84374f8612d99804d8ac399385b679e4e512f2e5fd9" },
                { "ja", "e6094ccebba055503f7f3f5a8cd354a1c2dbec21f91bf5e1240ba46bef2fd49eacd3b071d63ac6db823c245cf9e4ec45b5f018e750252b2f75e01425a5920f3d" },
                { "ka", "201005fe165cac6a6f8492ddd3176c649e61e67491143be3b839f7589dd2f8120aadd8035a7b076c67f67ae8cc16a3b8a0c7e7b52a5b20b551bc7cb38cb48c93" },
                { "kab", "007831d905961d2979c67cc9918bc787395d982ae7af344233b03c1d72faf39edf634fdfd85db4c4d59a5a46922c82070dee21d94203a4c3fa2984388fb2a8dc" },
                { "kk", "22d8e2d40c0b620e9b1b407a8d40a3b766b808033ce4d6650c55ac8f7ae317ad85f965c61b64480eb0ae333e4805ef3cd5bd5c99c06fdc2353e70cf082e2c9cd" },
                { "km", "ba4da23aed3365a43e6d31d44c7ec1c2b27527d1caefeb3f5ff71f589fd119894b8a3dad5a855b2d762f713fee5d4b253632015f5d066e1838b2a9fd5224e86c" },
                { "kn", "814794af7ecd8ce73377c46bbfb1f3f61cf467e09a582fd00ed16fb31d39ae6d5179a65c4b4127e1a7fbb15153f927aea60ef889d5a5c6ff7f5df35d61e4a837" },
                { "ko", "cbb175de0b52cdbd86f3cd6e96a451a252311ae8b7cbc572c35b12650fd294606562fddf1289768973c3f260d600cc72878d7fdb226a32fcbd3beceb39a7189e" },
                { "lij", "35a5bbc77543f70eebd894b6343b323cd2baee131e1a46f182d50f3b85250f10c4a32098d8184edd1158adca336689020f4464af6313bb1873f9be0dd84413a2" },
                { "lt", "9e35befaafdedf7e568add0ef23a20b3a9ace08197be4a0f97e1095584580b47241893113547001bb5fc5b1e9ebaada6abca96147fc021889cb09042fe2f318d" },
                { "lv", "dd3ad625c94c4527b2b45c4599d4169542842c6ec03deff22bb6349f234d5efd601d60a32b5224341d44beb1c4e38787169fee8b066b884086cbb0f24b507fcd" },
                { "mk", "15f56f9be4068c76732d92289bcd747678f48842beaf46253866c0784d3fbc3df8201fe2ab2fa20bc9c04767ae7e6c5808baf288e96ce732a00740539cd9e693" },
                { "mr", "29d5ae8a355491752b11d0ebc993a04230882f09fbe37e39a5dd89d32f458ffaf42eb794139f8d17c090648901b4afc24392580404db9efbce64380705fd9143" },
                { "ms", "d0a71b0f777a12518cfcf4bdab9db28fd0374dffed23f9200e917b44ad445c97090c803ae2874dbf0747451c085546174c64c2b2016b12740b812c8b553c73d9" },
                { "my", "1e8fde592fc2d15025d31c2a94d23475706f44073bb9b6308bfb9a2cb888a46c7e704c48d88a5f3bec46fcb98ee411048b20eff2108a583ea7fd51021e5d099c" },
                { "nb-NO", "26751485444a2d37d06017b8e726c9987f8a22743985e13bdeee7c6625ff36d8aa03c406607bf39f07eec10dcdb6eff618fe432be64cc5af1438b234d0f23bc0" },
                { "ne-NP", "9d71867106a51373ae2e9927b67071a9e66896aa0287dc372234573a4991a1e543e4f5f104047aa55b73f7cb704233d6e5e53972b4f97279a51065e903676453" },
                { "nl", "98065889e01e5cdbd26a3dad04ad8188b3ca881af17535e25eb6ca32097e79b1f069db04dcdce3ec902e4465655870f7e30f07ad81f4cccb1c59cf4b4e37eded" },
                { "nn-NO", "8fa0465994e24218b640b046bb15d6af3ae3ea6b8dc58daa05a648de65e58d80a288fa616f8f68ba5c47b692938bfe89207155dc8a7b033105f06e33d37b1dc1" },
                { "oc", "d016bd53b051c57552d0aab7c66169d744bc5f6e0d141cbfa6c0f63cee7ee25d1f233e427ba8b242077de67314aed4bb0cded73437b5ab241a31319c8db1e488" },
                { "pa-IN", "8df95ca6b1063babe6c134af8729be59513c93130416b4cc2036b27c3ae7b18ee7f1841fd5b63ce030a1e348431fa66aa7e17e9c1ec2ad5246ee58ba8b4368d1" },
                { "pl", "3a84affba4b2e79682c3cef420943a5269a186ff33dfe1657525117cb164fad9cf11039823b6814111e0ffd830c0504f3c349714a103b76c86ed07cb277f26d4" },
                { "pt-BR", "8ec8cefb59cb9310704c4adc2bd9d87637c39b919dba9305470722538f56956fc7d6c3c86362d8fde0e035f55c6686554a0e8e91630ed5cfd7685d6cc18bfb0c" },
                { "pt-PT", "72996843789d923d441a07f22eb5e9aa7de1a718de8cbdd9a200cf0bee439eccda98666fa125fdd76cc2b942e9c6b323e1df451ffee350b6ca3294fea107fa4a" },
                { "rm", "9562a62e43af61693593b0db54af2a2f276a813f76c215c6c7087a725fbedb2a52747c474fad5c205bf00113a3e9e9e51f304765317f1933db3000f33be70e4a" },
                { "ro", "0a79cce84a6af7dd823992d62afae9136fa8fa37a0a96bc23d2d2aa0bd0e2d80c3317e9c4941fa3dad5ba6b15ec06a665b460982b4363b343cfe846b0ea88b13" },
                { "ru", "639fcd4e2e82220b5936db9c57a23229aeb39f05f650dc2d6478583a328ef644aef7655ab545001d16492e1b64fbfb7ff57a388f186456ad44ce8bda32bbe217" },
                { "sc", "8942b45efb6ee736f2d502214a5dc83b9f232b82e5c602182fdcfbf0e157f683ff3867dcb91be9ab17ff51579e7be6b3f469330fb739845ffd6a24e42aa2f2ec" },
                { "sco", "bb18702aa5298cfe75fb0639c9bfd4b418aadeccd78e17a54dedf27178a0f0f6d1f3075d4a3bb8b4872c921d85e6154d98680b41c0f7d8889fd80b86041855b2" },
                { "si", "28394690c24d0db6fda65c1f94fbab0c2f578c5fec174ab25f903a6d1d722633a5c97f10208c8688e6cd7e336a09ac3cd081171dcfad93e753e3c451e5d613ef" },
                { "sk", "76e8fb065526b4670af774ee691406c912c66a78fd6963652d6edb4ca87b75986ffa1ec72138f98dcc1d4dafd478021c32e5109e64b58f273473f60619261aa4" },
                { "sl", "442f02f92cc005c91f83fe04b034a585306605d44a978c2fcd07494940fe93bf39337ab3b79610d3b26893214d8230873c16478d186f3b63822a67f6e37e5f8e" },
                { "son", "2393803580e1b2f2f18a17bcca6ebaba7a2a6b6189e37a6c61eada16c951b21b28a055b64c96f0e80e5ee30d783d91cefe0a84d0065c7bb6da0fe0709ea5abce" },
                { "sq", "3860503b57bd1c7131ebf0e7fc7ae3a436c7f367306618c038ebb276ecdb34d47b2cbe0738ee94142aa4ef93ed9d3da7efd457fd317598228cbf919414e46792" },
                { "sr", "6a702d9bdb946a44a68d352452f1fb544ee82651e5444424494e4393cdd75898e0be49b1129bc7a72ab8f284fe115034785017a04b624155c65d39dfdc627354" },
                { "sv-SE", "06090ae66211a275776f82851cd17f2619eeb2779dee21f0fdb255f814ebfcc827cdbf4bedf788a0219ef9c5dd05cffa649d6cedca94ee10a5cdd22ebe65b946" },
                { "szl", "324868ba6d7003786d89265bcbde621fe43305a62c15fb4c105231c80ea045c309b72dd2046be86cc9cee6fd412f4091953119f61d0fac800c063b6a46d523eb" },
                { "ta", "b3207f50633c3fa0bf035e20aa8e5ca9f21060f05fd1704348393e1e8d3a265f8f39bea95a0dadda62074bf3724146c8abe25f2fe063b6b074cfbee5adcf45c3" },
                { "te", "4ef81db354b1128595f413b40905247d1db648b32f7b5dcc706a133e903ac10d53777c5bb01eb522f998ed6ed156d3696a190232af976809386321dff16090d0" },
                { "th", "d931f04c65c18fd53890f1c8cf3aea04e03e19dfefb6c17c5814e6bdef32e2b43066cab3a2560e7e2a4532f865793fc5f3b0bd4a99f7088582f948207f9ba255" },
                { "tl", "b9a13e60b17aa2e274367df37d3f58e6766fe350b1b3b41795f929db8f9f7e054ed9aea393598e24ee5eeb0451cf46689ad8e49d775da288609858aca9c27b50" },
                { "tr", "5c36bf2a21605e17bcf5ff0bef81f379348ae18352d4309d02276863b6d9d5c1afb29437d26ede3ae90a52528d7385bdaadc4189d7b22ddb7e416fda08c539c7" },
                { "trs", "4be59a6b90fda1116dbfb0f7fa69c995b69663a184b896b4e453979bdbc5b9763f712adbf07e9ecf577c22d005d58a92051ff568b5fda13f1c5bfb87caa1024b" },
                { "uk", "1235c0378d0fa630c4f2add329c128f6a5d807903dd239aae3c616938a9863c7011c1a951646484b8cce1265bb8ec1c87896d71ca097bbc3543c702c9e696104" },
                { "ur", "2dacf79d1cbba3b58b417a1509f2e498c60b55fc872eadec47c9fa732be8e4ca107bedd4b0bb73ecb41d9e7200c16225e2adca1a24b5c4432677af32ba0d4bcf" },
                { "uz", "d47a3f2a45520a8920fbcf3bb5e5752fb4b28851e6afbf68da11bd2be622bae48d77124b465c4266671e027679ed1f5a7f124f781a54ce0cefcb15b975e77055" },
                { "vi", "59db756a8949797e589c57054a0c4a857e17a66f0e363117cc31cc427393bd1be9aefa106de286fd7782be234457fe60591cb9bda1d0217249f6820d5c43d56b" },
                { "xh", "dd7544fbe23dc8f2fd9f6135bfb6b98fcd5b896797b388832894e9fe375481fc9a5cd6614549a26362a4f090cb02795a78176764c9a2bf1224dcfd477ed0c07a" },
                { "zh-CN", "5dd63454cc1eb393f16fa5559c150a5c10a333f8d1dd59de424a553254ca6e6e8e45d3c42798072bf9a80048f7fa54f6e124117775e8f7d035665c40a6597fbf" },
                { "zh-TW", "7d83cea7ee9477499be6308e2c41ac4ef9d303ebe4d84415fadf7d7afe917fe9e0d621faadf0dc046114267200b5a51a8842e71e2849f02c9192ea2910f8610f" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b3/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "fe60a060eaa556e74306f3780d9301124772937b0400f9443b80c343c53ca5c23fb8b04f694a337e9a019348a6b9e809c3c3281e7dd3c7bc2d476479532a7137" },
                { "af", "4f90bbab9f4d2cec79b8a073f6f56936fcca9da3728ad02d93c3acb7257cc5a6c9a6009f821d44afb0badcea0d84a9475ad1d2650f013d91fa4cf8d3700c99d4" },
                { "an", "7ee682fba7e49333533026752b13fba49b39c5076002382be1549d19c5879b9ac6c410c9cf71e32cf9a7397fca37eac367d0f663a1adddc40a9cd62e02dcac69" },
                { "ar", "537ad6a87f5a113fe9d9d64100784ffde735dd3dc330e47c6789a6f11a9fd08bc27c3ddb9a1c9bb03ee308e817e2d42c1c3e6eeabe504a0b1fb0ee4a4af3301b" },
                { "ast", "7137ef84f831e7dcab86b9db20ae384266d28de615db77737765b8930855088a53a8824ce9fd1ac3a6885a2fe87ecf2a760608efee0ed2b146e09e4c9f4d7b85" },
                { "az", "2703071ebc433cdb42d9b6f54ec925b86097302bab9a35193e23c9d9073b211c50c1f5f5e4038ae3155646eeb92f9e867dd5f3bb021fac92557d8ef6fc9032de" },
                { "be", "2d39fccf57deb4cf817d044b7a28b80f9dce096b6eece1c1c5bb06451b586484fdaa867d44c108cf63d06a0399e79c3e7f0ab08f90879ace292f475b84aed4e2" },
                { "bg", "c156a4df596e399a984de4c32a5f448428edcf8a94257d4bc81a026ea5f405f42f06c71b956f55d85a5d5a89873effa1dc3b7cfb65008d62ef862cd7f8298dae" },
                { "bn", "19165276177b27ff35fad54a7984df1fc8c2583a370480e356f2944e5e8aec3d3e80c21311aaaeb39a1370095782a14da4d052a4436ecc94270619ad4e47c1b2" },
                { "br", "eaeb114b7b5a56e31f6c3ac7d2cf884d32d5112ddfe2e6c5f1a6d0942b828872ce86b50a7b6780c6cab18c1bde40942a25a06093cf455932160a1bea841a70dd" },
                { "bs", "14dffd1eb6e41d5a40e205a2811ff3575c0a1dbf926808e3e91a43f26323260d2e5bf9ef788ec4f33317e0965a382e5cee0581847b99b6af8ef870bb94a061f8" },
                { "ca", "db556892d685b561b358fa6746547f8d298a8d8e7335cb44a82e7624ca9c03a885b475fa639ac65f48c1b39ca3b090dff2fb50e1eb2886f3a2b2c7d693ebfc69" },
                { "cak", "0cf0a2c35cfcbee9e05dc36cd0741deb38da9fc9a192912557d2bcead99362a8af7b74b2beb332eccea1738aa1e3ddc1af5e143439329847c11b902d82af2fa8" },
                { "cs", "086211f9eb90876b0e2ea0e4376037222ba66a57850a90f7790f358ebc200a072321dab59caccf0b602b90ed42dbfb9b800928b64a2105d016839a68db4d83ee" },
                { "cy", "67504cdf1524a5f11ee228c7e31af2562090975de27274464650de41341fbb3685d0b80337e044502c89db0676aebbdeb8b0dbdbc682b04b095c44930dce043f" },
                { "da", "9905c57d91e445d776f3a69466010ef5d69cc647b893c8d155b1b7399ba675f2af27faf7c5e4bf6739ed14c0b66c49d1229525c5bb1c9c1894f1ae01e84d09f1" },
                { "de", "95d11b39e5ff987fe6fd96c467f703000782f441dcae055bc074213ea8f3b0ec3a7e40c2fc8e760210e25262323fef4e7f0430eb8074cc175dc30a1132067d02" },
                { "dsb", "6927dfc0e6f8ff69e3ec89de2350b1c4cd7bc5d8619857136c0077a245009b1692d1f84e5b2d4c86bc18655f124fc1ea146903fb65eacd532f594e76a4943098" },
                { "el", "3b5626a9ed22ccab4ec864bb1bba19d0b15c2bbd5c5dbda6fb8facd2667ea4dfb27078745c5bfd7b444e4eba0aa01dcefa8a4abf7ea81615ac976dc5951217c7" },
                { "en-CA", "4200b6f1bfe6d3829eebeaaab9261853dbee6cc0e37f690f01a1940d66f82038ae61d33323367d161721449c675d456c29a11e36e9ef6e49043ba4030c66652e" },
                { "en-GB", "5363e6c8fb096837945ed713876cc9a3b5156ad2a938d740b196389420da4de23ca734f69f8da9c811790c1a5347dc033dcc95f9e2a1f5a74451c8c7d56f0b79" },
                { "en-US", "873667fb02232c517bd377bbd3cee3a62ea2b0c6662e46b3839278e736bbd85a38dba294276e20e892505221f8bae5ca9daed3e21e72977b8a81236169fa737e" },
                { "eo", "642cc7b8151f21eaab21efa2631304f4494b41b65698665d150b8f7354030915aada96dd48fb6ff72ce3292da5c0e96eb046183566b2e604450fe157186ea7c1" },
                { "es-AR", "0c9b3fd4120336d6bad4a604a3d906eeca2bdfecff00f6ff77ecb190894b92200f9abadfcb25fc534a23508e79faf6bcb5744c6ebec2bd394e32f6fbaf284ec5" },
                { "es-CL", "b0bebe92e23e1b8c909e470a69bc302750d6b9ea90751191e582479ad6a9b89297a6ec6680a6afd78426c61c6920fb61568cc29ebdea66a984bf5213fcf5e28d" },
                { "es-ES", "1dd7e3a6fd874044120894d04f48d4443a6ea7953b5ea199d510268d5a3bdafbaaf8c58054925038d31e722ba76f6c1efc884f38f7de3eb3a96189b588e1b699" },
                { "es-MX", "00d990d8ad07ca80632ea1127204b53cfde6b2237bff443306038853ad2bb1b332a919148e55623dec43fdae21ccd1bc617ca914d8293dc1dfc27aac619d8ba2" },
                { "et", "c87cca32a009402684feee3830b51dee584a7be6afef157e219924fddd0e71067840cc19b7067f40d65cb7f3f3e9b1e574b04a305cdeff0b7fda863c35657d08" },
                { "eu", "56ab05b1b53e64b0ab7be09a2f128681f647b3a0db6999824d88f2e92ab5b810e6ad8bb61aad9b3ae4acfb5d49d580a865c53a384625057067bb31d979f18278" },
                { "fa", "d293d8e7e7b81d8966b37267ddeea4888111fa0037a6e8882a0b591a0248d36423a31f753de97c1f73d6d9132fcdc6f49b333ff01bb198e40737580e74723d92" },
                { "ff", "a5460ba7eb5b81e59f1e4d4de82257488984038c357eaad0fe2190a87ab9204e50b75f215b37aa53eebbfebab746e7ab124fa1af49051eb9c2f8ffaf7fdc70f6" },
                { "fi", "e2e44d77eb1c354f5ecd144efca1697e47c3de160f24b81bb2d10493dd0dc27c91b554240c352d0864cfec173aaaa1c2a60dbd4d30432e3c26bf227b00df31cc" },
                { "fr", "f6b9f3e38d0c3f603ba461ecffda69deb96c39ed7762b111cea7eb70b8785921bf18ac824fc1ccba661fec1ea4187e011eeb33e0a56efe89a662169d759751b1" },
                { "fur", "bde95dc54c3c7b555c9b983df286002ab4eea1157d96c9974d5a7d8fb9ce05af6217be6e9790a7a3906a775f2bac01b412de4db1f8db3557923adc5e6995b57f" },
                { "fy-NL", "c8790ab08796609741771b3823bb175ac06d4984c0eed804aff7a9d90df7c2c03a360ce37e6652b50311227a33c29439f6ba1588d1e3d64068784afe616c5f5b" },
                { "ga-IE", "896167e82af573bef4f11de0598ccb7f7b0121346b20211536dd7b7f09a81887a6a6b9c19b0911ef376e9ce01c5957baf772acc1799dc62799b774eabf7a0a21" },
                { "gd", "e44f699a1a05b1018641be877ea009e8615fc49ccd138982c94ce04f579b2da159cbf9b0eee7c038609a67bcc4343ad9cf3e320d875ae3a94becab6a34d7eebd" },
                { "gl", "2ba1a1f42c0db681fd219c9806d1bef53e10d6db4d1549bace1a3ff65867561abd4bdef966e4aad18b54a6b5c9208e95022f9341efbb10cf946b585800514399" },
                { "gn", "053255938730c02f4013549cb56fa4cd2c07ff17cee0b993600b7d7cb2f54370469ed0a9bc046a3f0bfe274994821a9765713d1823239848c49a097e68bfceaf" },
                { "gu-IN", "5c50695478b9da08efb81156f2f252d5ba12b09e3ac7900e2feceb623f1cc29f6f38bcc181d3be673071700cc97a947fce04cd5fa96590617b01109a2e6b5ec0" },
                { "he", "bcb79622c84ad838e174dae839c1cb11d554420545934cf64e052b6c07293ff4c77ff6ce3b9404ebc199e140674b9ee455213dee456715a0580d053ea71e3c39" },
                { "hi-IN", "d6f8c1a23b53d6cbdd7f842f74019914a6589d0d532f7e178c38b40f681fd58b6f2fe8b75d40b861c48c55e5b5692d5737079d73ae2d58d5a8159d9ca90cf8ff" },
                { "hr", "3c67a9f72af7e5520339acb24eafaa02548fc9e5007ec2507baa45a0abd08b236d3a92c88c08292d011c174b087b582dca514a162819495e1c01db0424af8b10" },
                { "hsb", "ea7d4993eea8d34ce090a1a312f1c084ce84705663b200f84490a6f9044cbc38e974eb2e3e3fe2a1c96e0815b3e393351cfd328b2c4f9c9a6f93dd532574da48" },
                { "hu", "d7877b698d576596bc8da398dcf3df70bce334294611b1e84838f8f8fea08d2d6b03049b98d70253442a937ca5b819df1b7625de3b05c482a52867f8c912474b" },
                { "hy-AM", "78099ef2fde8a00527eebcd74bc8b5fb8868daad8b4a627297b09ef0861879b3fc2c584788eb9d8f12c010b0166e849a3452f995d6a7dd52e416bb7aabc187fa" },
                { "ia", "047e5808cdec5e1b11498dad653573ef44d68146d860b885ba115617202feb8266a813e932dd9f5d367f8d8ac13d37ee31830a1cc805882cfe538ad02c1e6fb8" },
                { "id", "6831e1c5485f3684f83bf06ca828212ddf599862014be1ce91138a789fed8778d03386c033cf8e834750a05f8dcdc75f54d7255c1b15bb91810484dee84ac443" },
                { "is", "4f004094b5b096cf7672d6ad991472a6b803a8be586ab9fba4bb7c8688c83a9d569ba12be2c9ba253c75ab06bf95e9cb7be84f2cb9cd3eab3f4e18f365bd3062" },
                { "it", "a38ac7d6728144a5cf1ab01fe80a6f4c4511639a7cd511bd4fa39e679b018690b044f50bdbf5f763d84196b91058f9345206a7588250cbc83de1d66196ed48a6" },
                { "ja", "ca2fd6883b9a2836b520ed42517a53e1e1a0e79152b187c99f3d4a30db4e4f3031abc3d5826af257199eb70b46e8461a1e620ebe347d5ee584fb9e90177bced3" },
                { "ka", "64e48f9a620d378357ded2460a4294e8929efcfd51ff1c1c4f014618f8f600fd236995bea7b8e64ae99f738fcbba675b60aae390d6dc552de3028460463ff137" },
                { "kab", "19528aff6f2b392ed20f47fecfbcfb6b1dca9f943fa9e1753c216529425862532035a8cd14aa2a35553fd36e0031c767c399ab6ca1c4f6e2179dcd4a76af18a3" },
                { "kk", "e6b538880b8ff8bc3f96606ec19a94648cef4eec9a3a46debccb0a9f5eac840cd1c453c741087a53c5d13c51869c5788ce94498ce2424b1e3fb61d6f9da3d56b" },
                { "km", "ceef5f0ee933276d3a48f6f473441c5273c14c79832948954f65021d602ee9fe1c8719880e2264f0882528e2c891691f3a859aa5ff3f672630de8fb9e8dcf231" },
                { "kn", "235315c4f83e9d33d4f2606df7fa7ebe67beaf44ea6fcc92ddd2c2ba3b4cb7f394ac7bf69e08bd059408c091248ac1b354c876ddf19867e07dec875f2c841e9a" },
                { "ko", "1eeae68b490c748fd38014182caa13234bdb8c147d73b1cfcc9b7885ce3b1b8c13a75b2e1586150e5b27d0dca85103cf3b5d6ae134aa72cdbd4844542929765b" },
                { "lij", "3baf7a103ab476a464dc313818acf948d90c36eb3524c97e7b42d219e1ffb9df3d9c2affdc187c413dc3fb20928b8e051b42dcc1dce60074dbf245e782166f2e" },
                { "lt", "cce6e9b6caf226b5b1715bfff69cc741b026e6fa56b394caf0f200797eb3748fe151542a13c1f8fefe457dcd25026a92d3850c382c667db002e4d2e736a6770f" },
                { "lv", "941c140c2e090800fbf8a2bbd7192d54237f29f236f0e50feff27c8c9673a00d64fa77ff85a5e029cb65d58606ba6c7edb92f33d8bb924553adc78bb7b7a5637" },
                { "mk", "fa1ab48b48980b674eb5035d3365d89500ad753b4d9dc2740424ec29e5dbcd248c63a9670a36f85d5fe1f276175a1a399fa846470aa09e94cffc4aa0d5eb4149" },
                { "mr", "cdcf9929abc43319f4bf8f8f338a7db70d309a6085250aaff6d0b4d8ffb6914e9f24d430f9da8bdab0da173f8c2bcfbc3ca9dd4914f96c963ba350c906303f6f" },
                { "ms", "93010afde74aa9ab939113a332c24ff6626b3604b3f4be03e2e320ddac5c149fdd557d9efc1296861e772054c880189083c8868a7da10af4bc771ad36c80e1d1" },
                { "my", "3745956e491a53e52b089482533087a7b1b46557a4ff83421763214df7aba8f32948b2e574599af5f800f359f736c2a9ea9e487e2cb2f2f8ac2eadff1f1623aa" },
                { "nb-NO", "8d9604730a61c81ccb69cb3e85f9f814c5f74b693b5a7045ef463354accc3b8fe0f6fa4e3bf6b17f4c28a50f81d172c31eee2bf11040bf6d31d6198d7f519cdc" },
                { "ne-NP", "a64cfc009b2cea9b3f609dabecfefc559ce3636463ef69366a2ac6e7fd2cdd54eca99ca13dc34ef567aa560796efe8de3f7d62dbd271075e274d2aed4da9353a" },
                { "nl", "9e003fb6fa64bcc08a64136dbcfeb2f2ff889356a29b69a02c981a4ecec6027af2ba2e8afbf3b35a30661141331f788e99a2c9c77c1322d77754d30c8afc056d" },
                { "nn-NO", "ddf86ae32d186cbb0b05c737d9f0f9bd1083b30e97416347ea3b55947b8c9e064b8ae9ea72883f7152af51e7c8e7676d83a1280b8bfb347ce0f5a82c686024ed" },
                { "oc", "2b4a387d4c51c7a99790910a8a95795d876025120816944a3830286fbf47d8f61eb631ad34b08af3a565fcf1659bc503afda68ef385d5e5928e791e8358dc3ce" },
                { "pa-IN", "09563b582334d8f8cf15e4a93a359f6cf00e3051e057335d1f73ffe5e036fd81d831e2d43d1a6e78131d552b9c4032190a85100b93255343a63c7b1df89c44b0" },
                { "pl", "b37e9defd0d6820f2344ebcb3c142aaedefb4b02f66e6a8523360c814d933f524b75e3dcd78f2bc02d2d75ef7bad76c1fd4657d482ed3c5d8f125496960df24b" },
                { "pt-BR", "b27e73233e0048b35c4c96853642c132ec802e62d5a7349e8423ad2387d85a4b130622b41a46383dfa4a3579d8c027de92e726efabcbea167886cbe7040707ee" },
                { "pt-PT", "bda1300002d5d3937c87bc9218d8a68b0d3032b0cf7b79c9a664d22e97126af81512761766f75e197087a2b10b3318f78c29f2b750ebe3e04a1db955246c9503" },
                { "rm", "212698d2f4bd062f97f1b47108bf1ac17213993da8943a3819af0fed66ecf4661bb03716b3e9d9c2c6c588e0b24d4f2dbee246d3510dd9d3f81775e2d53f7e22" },
                { "ro", "9a8d420094311ec1faeabf728e1bf58bb7d5e64143c79b1dccd42eade341d925a2b3ccb11bb0eee6f5923dfc46ef223d4f893fab8777629f3434a1f6407870e6" },
                { "ru", "16069c1a37160d22431b72435f17696ac2cf3e20dc2ea563058301554b2592f2a0617be11aa31b9d89fff0cffa97b094efcf86f63b7ec61c746fe604c6c5931c" },
                { "sc", "19396ad1a006fce0d56e89f6e0580930d80ebad07bac86dae4b828817575080752cb8adae6aff0123b0b71888c71caf2e93c26b99f138778c8ae428988f2deed" },
                { "sco", "315ab4b31615a933a87ce5fb9787fda3447cde491b082ef1f3eb06be758fac8880fc18bd48c868ac521d64360361cc8f1ba708a7ee53a38fcd77aa8168351bdd" },
                { "si", "475a806f58dc2c2cda6bd630c6efb6f50bb2ef6cb7ea9c057242f458b0b2172ec90f3a8634f6cf59e053014ff804c9bcc820a997097afe1d7b9e8466b01120c0" },
                { "sk", "d1e90f25aaee57b0db73e0d0e0a28075893f5ae68d40fffb878f1d050d9385712d2fd2d898f04b0a9b92c297c368f2db3ea1f91506a95c27fbeea2ac0b6190b9" },
                { "sl", "bdd849f237978184873e141e473fc89bf146558c68762c0e1cdeb77265fc58c5e2a3b7fc15322920e1a2986ce99e15d7561b2f4999306fe5ed6f970789c94cba" },
                { "son", "07027c2c8381010c11875b7986288d81639fbefa8c3429d16ab203436b50e0c091f1ef60cbe10781561786ac59e41d8643c583361b0c3d6a65f124dac4c109f7" },
                { "sq", "daf43532046af9a773c5649053c20613cc8298dc606593bb827e088967b6b38006dafab64cda977aedc4c407832a0e85177f515246959c5c5ce841dbc4593889" },
                { "sr", "194e5b3a58689697e8d0c0521777c272e9bed99f7e34cf40999dc0e68de8faf1bca92f9c454263412687cfece5edb0295f44899ace991f85246a42ffb16aa514" },
                { "sv-SE", "26393da7e8b459f2bd57e031151a368ed7c351b980f108a2a576281cba0c1d7e83c450b0b8f795f67be06c227baede2d0d41f1d2c5498bcd950a31497dfc70ec" },
                { "szl", "2473d75919291d5ebeaf4e9b4f43fb5ff56e1e1a3acbc1cccd8b7a6c629efe4fd071d566a567eb3b16294a3d30601e428da7578c8eec3b3ee0f96ce898c09dcb" },
                { "ta", "c73d6388e8e6e303c086c0e64d3b12db521c21aa84824c9caeef89c46566e0cdfe92bfdfd8bf93c2188d2b541a44aa798872cd2b3afc286003fc07a56b0862f8" },
                { "te", "2f050249713a924bb40f38c0596551de16b61c8a9384d3e74cefeda13cbe6edb8cee34d3cce8277ac4371bb1e53640bce957b29a85c44145a0feb16bb3b5b912" },
                { "th", "f3d3b43631d7e246f7bda8150b8bab0bdb97ecd79718e231a7f9f953b3746720edb360d754e2056ddc23245161d38cfb1907fddca92e106290c59b5ee306bdb4" },
                { "tl", "2e803921954d17dc89251db2da6eb32b1101ce7fa9ab56433b18fac571cb9fe150787dc7395731f37d10e723a621b39e24ea4547003e368b0d33df3296693997" },
                { "tr", "aca93612c9480c042576a0ce73453bf145b100763a201216bb17a30b2011e4bd4de3c2741ec4293cf1d0e01655b5b38547f32a5862a5e6d0e3cb578b0bc31137" },
                { "trs", "8da9a5795b67a3b500fcb9f8f33ff120795d7d29000da495d56a6f85763fc2abe530303ee768bbde463fd72d67db3e6aa2595bd047566b8a2e0f70c4a0dbe345" },
                { "uk", "14eaafd7a94fcf00b27bd3b066f718247fc3412a9539b8ba04c8e49844355f0859f534af20f8e32e14259e20469aa7d1c058b520dbec5cc0061c972148a45986" },
                { "ur", "b62bb4148079974a07af8f50761be3065d8b4bddf3f1d1cf7c9c74643e8961c07f75638e47cde5c2038bc2fecb3b2968dbddc07cfd0dede50869a2e86d03cc29" },
                { "uz", "08f3e307c2c15fe63de3af87ef50dfa172ea97f928756b9f7bd30ad01c13d7d1e125f940bfdff143b75f1908b4c099592769b4ae10a609eb91569c767c2a394a" },
                { "vi", "e914d22387bc65fa05804a367bf6d42f42fd0e728f28ae5cef7e9107c32d0fc7da3bc9ddf2beb14fac049e42ddb11af01e3fce7745677bc4029ecbd0620a8015" },
                { "xh", "679e05caa91ff9ea3f5d2541ac73e3d563bc81bc7025f44df49ffb2adf3920ba06f393babb1734c7e00e55988a90f8ae3e279dd04c1d0b3aac3217edb739dac5" },
                { "zh-CN", "d2489ff5936d2bae7299328d25cba60b7cb397ad82dcc8f654ddd428df4cfe0e755b8005fffe8a64e886521c15b7c91432c2e12ac77452a801325cf27da97c90" },
                { "zh-TW", "904d22d707de87e92c773f1a49efe9c69c0751cdb6edb7b984a7bf233b9cf00ca5af067e067e62fbba6a4226a7a2f6d5b6d10dff7a32068063bed3da0171c657" }
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
