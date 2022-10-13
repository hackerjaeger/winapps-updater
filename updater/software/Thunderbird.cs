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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4bca88e9fa1acef889a79553e47b4828dbf57c651950d91c2fd9a3576c5d14c1a5db9f862dcdc843e74fa3ea639c407269d42f6b230937a373a927f286e1d415" },
                { "ar", "c71592fa10051c0827525aaa6967f8907c126f986df01bd2832c1aaaea69023ac252a4e1b637b65eaffdaf16ebed9eac88e3293ba786f6dbadeffe070f233af6" },
                { "ast", "eeaf86277febeeab0444eb344e1d74ccb68c0b4c8a11e649b82c441a5d27206e59a17ae8a5eddea3ff6c8cd096c325b3392073ab614ad5767a6477176e594165" },
                { "be", "740f3f75e77818cd3316fdf73aeec3ded84fbed112d280ddb3b3af4fd83b0d5a9d67adf38529db54bccd5fa46f97205977bc084c270b1a37e5e4150859c2a3bf" },
                { "bg", "0a2f2bf12f77ee4e2909946c7800aa94d7d945a659c86b730a72839d5d0bda7225f5af00496d7ed2ee68a20f703cc0ab45d6ad85775e4eca6ae0871fb549f50d" },
                { "br", "9be7d40ba7dd3b631556b7c1610463449f3ec48f37f3361514fbdfe180dcba8687e0feeabaafa273953e315378d4d9effe4854a22bdc37f7029016a3de99c889" },
                { "ca", "0c066ff9217ee50edbe8f57c3e78222c4264269645a690f4f7ee81b14e68d82ad3a4487f5fd5293f520a486104b82119a7db4aaf612efb0da43d08542d74a383" },
                { "cak", "e2aa10ef5ae7b095e3f30d60c8cd459784708383209e22caa8c47952435bd0f48bedb799bea1ef237f2fad6c3a297d9f66d344d3650a87f6f1d46d6ea97b555c" },
                { "cs", "33427bde6f2fb17d4401d10f8c8b65528b10430ab7bc00296809b2f3754dbc2ccf95a0f1bad442e5f7c7065be6645406bf95b3aae762618e83563e1852ab4719" },
                { "cy", "09008e3bee19f0cfefdd91bd07dafc9a8bc42f03f9106bd79127e88e98fe63a2ee844f1f747a1857da9d4a4675e07bb1e0113da2e1f759fe0e910749c912f5e0" },
                { "da", "303b6d9630fb68012e261861e8578b05cbd74111955cfd36e5f87da16da586d4f0e2af1fae4c269e77779533818edfe78b4903733b95083a4e50ae829e1b3518" },
                { "de", "bfd1129912c591d72a6b278b51e95d69477a72569148699441270bbc0395d8feb4e8303254284accc45a34373e93fcf2f75184b93aa1217f7310074bfe1e3992" },
                { "dsb", "d175053475358d28e5d657aba32ec95680b5c10df4132df5868aba42f50467c0f2a30bda564043398135f2e7ac12bcec196828966807673d7a24f4a976ebbe1e" },
                { "el", "af45ba72095e86ddc106e006f6350c6454d3b14380ebeb9f790d6b04712a6896e9b7a3c1ee3b43a98fa5642e6c3aca873559f5c8e2f925b4b2269a44cc72c9ef" },
                { "en-CA", "a459fb530d5c27be503dd93248ac6e160c8205b92f814a6c2f1c1363b14f6aab15a6fd693ea04f6963b7a66e92db8088a01ae920c8119561e93373cb1b6d01bb" },
                { "en-GB", "c383f75b6b8f4904f5832282cbf25db910502ee07d8f08ddc2b308ebc343875a3131725df01a53c261026f8d36c388b92876174388210c24fc2af996acd439ed" },
                { "en-US", "2114bc1783ee811aae5250498ac7785a2ab7a11e7bb2ec9a3365e935b54bd1456aa6c0b9ca15cafb2ddeabf8e4c682f2061abf05f1e4cb0a8c005efbb4e1082f" },
                { "es-AR", "ebae3bf51301af5d12e423837ba9cb9c21fdb954704518d8cbd25c3332a06dba1d08dbe613d2ea98ae960db10ecf2354363886560cec0391f7749bae5d28fa16" },
                { "es-ES", "e1d37cfb0fcc4fbd7c5e5a20d54517a8ff36553a181ba9394171e477194367eaadd761a7c572472ecf7c5166f47c88a8e47f671b2a12a971919fccd71e6932c7" },
                { "es-MX", "400b6a6c474f17f0d2344ecb11c88421587dc64ad81b3edfea8f16220cf4740865117592c5ff41e5295f474b3b725bcbecf40c62299a7060090b28c84182cf5d" },
                { "et", "7c5812161ff841c3a6e2cb19fde63ba1f24d5501a3f69f2b926ed930c488bec752140999d359320b2aff2410d1d0563ee287c5427f03b3c638de5dcf5a8f5e54" },
                { "eu", "79fe5a1b40614a4ca3c01f857faaa615594f22dd47e8a841b5523f65c704e291825ff59825e8a2cd26dcaf9bd32dee35600d3463ca3a708a0ce687382d2a59e4" },
                { "fi", "fbbd92f78848dfaa405c11aa6e1be0940817cc93117e046e42f15425982307632ee54b4f727136912c16571b96805cdb7e8d74c950a42871d4e4242cf0f57d3e" },
                { "fr", "1accef576b98cdc5f2fcb43b84cad6a858d423f9bd3b52e3e2d2f3d3d68ae1c0c6686359b91d0f76411b90049dd69f1c9b4d9bdff7554dbd0215cc65d57a36a1" },
                { "fy-NL", "442060011d0b7b00f2dd02eae569b02b7793975043023ee93351b1b2d5f4efaf29159e0187b7c6837bbfeda8573c83e165dc59871fd62cf0a369712dff11524c" },
                { "ga-IE", "82a723ba1874bf9c78fede3d895d813ebe49a46e50dd7006f2ff6caf112f414e2e0683d824ea747e327c2f6476561ade166117aac7c2ba14f49a50078d39f18f" },
                { "gd", "6b49eb93762025284f7157707b63f6c8c3aa5790e0d30de64d49440da4c98f8b7f82cb3658ce3dea2e313591eb6f7920b5c8a2d40ca1420e19abda518f5e8abc" },
                { "gl", "44a1a5bd4adcb4a0f1427e28e81c0ced779a82028e2500e685787857d9ef96d18ab7964e1f9a7e887a68da11c4fb12ed718a522698e92b39986b90fa06a3166b" },
                { "he", "784e728b93b0638ad06bb884fc33bdc271c2b1837dba06ce70dc81b217dea04e46f6fb7120f2b307fdf4c2ba747f7fb11b61c08aa6f073f4f2923adc771f4b66" },
                { "hr", "f0cc65dca476b09da8263beb334deb00513b76102ff7adf5acdb171268e3aadbfc6ada3fd7456793d1c69c544e712094f4537fed82c61d5448ad8f33f7ceee2e" },
                { "hsb", "b880a30491e64d9281c657c7daeed15d26fe325fcc89475e947bd75a10519907b4250a9432f17ae0925d2c1f4fada8d1e890f078ea00156a8d6218d2f89e01f6" },
                { "hu", "344c10fb9b8d5dc5438dc64022714ca6390b21d8c86ccf3b19f2ebeccd770978e0c2fbaa6c05d0a071ec5869f567c60da786530cee309e62e7db4d410d9320e6" },
                { "hy-AM", "861977bb59b7430e695aeb380e7f0dedf1f0ccfa1579cd0695b2343d63d9c53d44e7be1a4b0fa38e0a5641f333850fa169ce633814420e7c8cd84cf416f9bc86" },
                { "id", "5fc04e5ffd6b45598cfd5849ec958712b30f40f8a5508e37d01819c65c20540796f2c7ccc61165e0b90a49376c840095ae2864b517b59beebfc0d43c4f9d26d0" },
                { "is", "ddf85b4b64ead954e53b1d5066fb65ee6186b5c7fe21ce77bb4c69de8d5db3fca9ff49015a59d163dc8d244fd546e76ea846a5f503f41a9fc6a73c5156128a3e" },
                { "it", "953493f1f77b054aebf00c6b8e3f703ed8741c82317319ee6ed22d88fe531d74c82044f72977528f47be7a5130aa73e15009e74177d336634960fdf878a107cd" },
                { "ja", "d1b28c2623de3e02425db757591d514c609453497ae9d122f7ca842d833efc2cd9030d2977bf70d6eecae08b6982f1d355be6991afdf25b8173832e8aa6a54d1" },
                { "ka", "4df022970eedb79ef0b2268f5ccc95d5b83756fcff8c236299bb34ed2d6d7b8f657a868ec39d36f0dc3abe02948da957563827fb7f1b7df4bbb52806974dc963" },
                { "kab", "6744829b946d3d5d913bed8496ad0e44928a69f16c284b8185d51fc6454962fd32d9c3ea6d1de9d5b9e281aa0f84ada66f9b46035ebd5ff6d9fa6cf335424207" },
                { "kk", "353f32adf8e6274ecc86cb5809bcf88c8e846b1c3ed06436ec484e43d077fae39b3d926a3390050e510996706e0dc7345963618eafc515952fe1a10a56961efb" },
                { "ko", "18e5def2863357921297e6b73e37278357f3df07591c337d870fbf27e5ad88062fe95e2a7b757d3ffb4aeb163edd0633efc2234846a66d528802cd5402c92a74" },
                { "lt", "31c2d2734498c760bfc8e8ff06b63adc00b5f452c679ddef60f62ae91b893fefc1c5c4334b4add2ca1b69270c95ffbad231713bf48e29bd3ac10c481f742060e" },
                { "lv", "cdf48a3958139bb9b07a7bd16c5ef5e854c47b1be80fcb71c4640e77566d8ecd3c7d296e25b91394905be01652818aec644f7355607a1ba4ab47c1215073b6ca" },
                { "ms", "7fff444dfa0d7132806177abbeee61f8a34326b89e3595585ddb543fe6467993163a65c8ecccaa9f33c9a2089783d32eef333a843f5ca7d6e5d8d40f2d88a711" },
                { "nb-NO", "a3ce41a718895bcccd5540ca2147fda8f49d2618c9d4487f5afef7734b72ce4e5c510ff32c37adf0ffcbe807abd912961d50f09fb8435316bb5c1e9d52124b3c" },
                { "nl", "d81101e4c4038796de3bb695ceea26d34a1451ce1840085c672738c0b8339c174711d0259fb2e865b407e946361b15f6ea90767e396d37a3ce1be63bbc454d5d" },
                { "nn-NO", "e47fc1f234ed2ccdcccbde7b5b9f57df609f74f86d16e2a9a52da523ad6915453718de09dd59ab28fb09d05cffae7373c1e29afa87c48823c2870d4ac199fab2" },
                { "pa-IN", "db262306e760a39ec5c3ede2040e7448c9b009e96303d2a1eb4bc8c6549be1432e19559ea6ccd23e87405f895204649461ed4e681b34d335cc1a75972d392f5c" },
                { "pl", "731ddafc80935a1d6679e101899b7cd762795a5e230dfb490db388b0a8e0f854f5ad998652fb9ae917481b009a5d873826327ff5c3a159617d6d28b7ec0531f5" },
                { "pt-BR", "0809a833beaa27a5d613c125e77f8f1e46322bbe92b17011caf545df920517db6663007eb61653c665086052ee44922485a74ab1241dfc44092143080cceadce" },
                { "pt-PT", "2d99b27de412239cf9d6f312ab5c7eebc2da0ed52f84b0bf6f36719831803fa2a9193edc67cc470ee60100fbf2b9fab5978c27883e2eafa6553147ef708b3caf" },
                { "rm", "2c105d536cf37b99008eaba89f4aa48cc40803281cb366d818cfa0ab8c6419106c9b69b3a055d9b1dcd95b729c114de47e8bf1f4899fec80549edb16bb7b232b" },
                { "ro", "c1e07e007b04541c2c068c68f0b641e6c2dc955d813a44fc4f6ea723456b21ecffed7a049c91a6a373b56c3208d01050a6207b1775bf136346886d2c1f50808d" },
                { "ru", "893b80710be6c572ecf083a26883ba933f151b85a7a79dba0e831ffb462d24c8d3affe41869ab586ce6c91f4b16888d8318480cc0e58ba0833c4b50188a42328" },
                { "sk", "a22412acb36dc3972ec2b46e1c0aa4c83059577f7ec3c94f3bbfdd31e0f1087073c354bf4ed49f9f0891c11c4e5ed420cda8a1b670439d3d9e7e532407859a32" },
                { "sl", "8407971e805e3a0aadb164708e8605a22812f60591b74e7122535c05fdd439dfa078a27274501e3caa958414a2514150cd167022838241d66e7045408f5b8041" },
                { "sq", "0ba96953417b7747562977855534cf6b66cc9e063de4f0d1385de530c8eef1a6c89c46c1eef749ee2a8b09c00fd92a7147a6b95da2a2c9782f2ef9debd56f24b" },
                { "sr", "93b9d5464842140beba5a962a0754ed2f223241f25cd86e8886e16de6f22fe08d17c927b7f54d63a95f6c6666f257c6c31fcc96b15d1fa7b16828ac3042d902b" },
                { "sv-SE", "9ac4d1c657292c91f798c2d485bb6d40538a4f52bfef91fb7b1301c09f79709a4ef54ff826c0099eb2774b3d2bcf880ea0b19d1a045e13862aa14ff37af6ded7" },
                { "th", "089cb68e0670c694b8a9eecee3490ffd8017487f7cb841eef5b50a26856755ae8fb37a9834a1bd01ac4771b7cdbbbbcae2651f5fb3e76f72d76dd50ffc679ada" },
                { "tr", "58e387cb69c69315d5e594edabc94762b6ee3ca3dbd0fb95f217e60b6b84c7fae1e8cef62b2ac566ad68adf3ce7dee4f41a49612c24fbcf8dec62804560b4187" },
                { "uk", "d6a890640fead1ffcf7918e21255f7754fd02de2098996fdab98ab7f0124579ec8e103f036d3a380fb0a617f4da2005db97bdaf1c705861b107655c32a111a3e" },
                { "uz", "61fe49187486fe95ebb1d83b9d5cfa9f7471a667e0ae4da4e2d6c963567b6f658fca8845fad99164807adab63cd6f56b45de0e3986b54e0b637fb1d019edbe72" },
                { "vi", "83542fdadc802384d408845e410b3e9cdd903acff4943c7991b40ac2dd5cebdf33aca00b055614b04d488b016757fcdcd9235e4737eed4357526ed8dd63d6101" },
                { "zh-CN", "82becec4219ea80aa890a96d1df001a8f84e4e9dd60645c13a52ac3cc42b90311582531a89f7e1aa6ec51f6d8ba23af95d17b8ec858dbda73178aaf5c593255d" },
                { "zh-TW", "ceafb78d35487ae32a9d0e898681a1d865b46335e4b502f3b7b5cc856ac8d3c7384c7877da775b0ef500bd37241e919c7f091644027744ec7a6a6d3536214b18" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "969672f274607d728b3aa5ab52395005953d8e5096d37ca22c26c457009fd322e998026ae7a8aac6288369f2b0753c378cdc79a1af4eca072962d77b891f04ea" },
                { "ar", "563df7a929e838da3819fb7919ebef9e32db5848a50d439ca15058988e203494025cd49855661484099515de376fbe2eaa09c1958ca9b9f1044b9905cbe02ed4" },
                { "ast", "502ec77c62458c9ffb6bb6d4edfc20a3f6e091824d677593bbff957e40572d1e2495e8e2432c2704300898e3831c207bc2b51a0304bc4d10c84bc36ae042fbfd" },
                { "be", "181cce02297c8722c0268e415f00c91a905c5b3487952c82786be749ba273af25d9ee90f023681c5b6d434ad4b771b758e038932c89520c501adde6be7167424" },
                { "bg", "8e12e4fa951c5a3c21e284991cfd29e44cb3ab40fef1cdb3fa915c12b49168663afb89d6057297547ae59a1d8138ae004d49258fccb42f2dd953f97067fc7464" },
                { "br", "aa5d31ff4d6911bbd94707cc9cbfbe376454ef58c148a11c2888ce5f3f092758d2498362a97bbb0a2c53629e73df79debb5ddbd21cc86c7aa156122294f821f0" },
                { "ca", "d367f225cedbd7ef895c11ba3087b2e786b509819f2731a9ec33f3dbf6bc454cf1da30f3448ac76e6f25fd81ce3a28bb537512a69af8e7205f8c4d417b4891b5" },
                { "cak", "bd3a5e738a70c0d35e9dac5213a9d315a56760c052c29751918a6d8dc8f2ba11a5cc1b03fd8c70af2fdbe796cd3d9310e8cd6ca77a62ccd83d53aee9b9e65f26" },
                { "cs", "2a4b83a1f11852c05ebd89e593f5131ecb5d093cfed89cd19f3a3e34c2f4e635468e61f9aa998f4d829013eb0043f3e0e15d68efeaf161b0ea8cbb8ece21c499" },
                { "cy", "6737332d2456ff440d16c6966b2872ad86e3ee594cc0f8ee9b7a67b27fde54e129953cf618acc65f0327b1f0fd2b82791352bcdd28558a13c28d0ffd5aa29ee1" },
                { "da", "37dade17b02d95fb68e36ac7432d1db26297ac9a416803d8ac4f4d71369d94fb0dde2602c31c1ab476e66754dcaa331293fc01641568c56c25bae6dc38cfdb24" },
                { "de", "77024ebd7bef58680674036b88b8b707dee19fd38445d86db42921d97b2c1f72ee296a4befdb92d1c9e93406414f666028abc98f7d67f54363dd06dcbcc4c834" },
                { "dsb", "1e6a3d9e42e0353c62fb5cbb866be064120a6c55de5f5e76e78351d2454f6ef9eab2bfb0869671d9e80b67abac3a332eae9ffdf6ae1866331d54171da22c7de4" },
                { "el", "b7336cc77d3c175c93fd501a9287434d43b3e6b6c88e03892239dd73566de282f32082279b64629d7869b7f7d20535c8fde3bb4e0465800adcde7c12d2078f75" },
                { "en-CA", "900b170f42cf895b741cc9e2a5bb0b8b81a1d80dc4558bc5a030b624c2a0cca92199bcbeae40cd05d890755b8a2bd901ad4765a791380d7a6c85f21b9f93147b" },
                { "en-GB", "d159aaad26c7e75d4caa8063a3341eb555b3c4f0020cfce1b77ef6e23d605e8b617613c6110e554bbcaa93ee0ee54000499c35f6063cca36e9a37cb17ea6bfa2" },
                { "en-US", "58f8b4755313d1ef113b1f267a1e2746b5453c2bb736e99ec81ffa871a1f50a101bbe69691e37d0adb8b2de691497a89901346fc73c0a26b991b523124dee18b" },
                { "es-AR", "90aab60aad5945f68221e3d7ba9423a8a272f5a0e7bf3b835c7dfca70358a00c9112c3afc4e069f9e7a3e9a35ecbfd038df4317357bc01c57f902179d7344d22" },
                { "es-ES", "2186723f3f5f2b247ee8659885b08e5d812655bd8b13d316660d3f69cfb9c0b69c29eb48e287eba6d47b20bd3ab486c6b3d1a87ee7b25ba5131a38540198b369" },
                { "es-MX", "d1b0dad5982a5a42bee8dff2e3a6e6db2e934bc77bd55bfbb4ea29608bf814c19c167c69ecc4a8fd6f443c73073700e0efee394574dafabe61e0f82243c25d12" },
                { "et", "6e559b284875c0ec91327df497273f8a7710ef556354529940820ae736f3ee91c5c318d6b672f0eb954f9125324ad6a2e4f4fda2764fff699e9d1a1072c65b2d" },
                { "eu", "b80c43d04179246207c318a17566753502192a91debc96604c353aaeb198d5cf56e0f87d87dd184b7a07cd83e1dc1f110fbd7f6d3780a57a6ab063ba9c57e2cb" },
                { "fi", "1fae6f0d46567c5050bc7723156e656c04100198835dde1cbfdb1dde872e54e2e70c7ad0272051c5a4c13e426998906474d954fec57b1aeafefefb72f9b911a9" },
                { "fr", "63e4481fa4e160d570a6d8770115368c624ca101879935c98f13d35120b65ee94ecec99e4b610088824bb793d8b6a741c45a53539e2b1e77bf6a9f6f433756d7" },
                { "fy-NL", "0727de2db2146c0de87a54eb1877f2c2346ab33c57a9c6a196806f635c75e7fe666ec152d44433ec3701bb56767c3933aa8cd527d27df30108c642d0fe4914ba" },
                { "ga-IE", "af930dd57a2ab7f05e1dea5cf773fe15594b69e784ef9103ac087424faa73e9cb5c5457f6e6626938f9a2e818a7286378c80f44b39d77559605a17fcb1dd7153" },
                { "gd", "4b0656124f6a79a18a11c88fe708ae41810206b10aacbbcaef3802b27bd5b7d2eb14903c05f5831b1a02f7df94e113d6cc16dfb2293688e6c6e9a8aedb09d0ea" },
                { "gl", "04b58250c78dff274fb38e98bc595b6c49fca6d1c4f52850d8d0041b5f8be2b525c341516d31f0c6987836983cd7844924f622ec77bea012d648cb6393d2dcff" },
                { "he", "b1a8bc625cf2100bdb175fb2e283123f442f7ba0218eccf856c4983015d5e241c2fbe2065deac7db4beb0f901fb64c3e5ba112d42ec60c1fe91c068e173e84f5" },
                { "hr", "d5759f255d3c22cb1965e310145feec3f9173b01d139581c9cd7143a0f44b5ba049393784a8a8566d1b62e97cba0921fc3e428dabc794b2388ec95a801859dc3" },
                { "hsb", "9dbdd1a6c7434d13ea4880afc9124ff1da6252a7b66ed6f790b06c166f7f3828ad66e4061096a97c7bac71e7574fe8979b09f9ebb774b5f7440a745cef20e688" },
                { "hu", "778c79ee843fed0f3e7db8f8d6e0fd57ab6b3435ad85188772de5905f3f2e786de80c1fc91c70618391029e31b61018b88f74341e24187663047f1e381405555" },
                { "hy-AM", "bf1eb8c50355e1f26ddba8c16bd943ae42e8e8602c85f6af75e2d14d207b9f7ed3501e8c133c049ec4a8e0ec32399a7d05f3acb2846b906a35926653ba88efa5" },
                { "id", "d6ef52e4e196b7a27ee82aee3f12d5a8bbbfa627901676d0176998950e67c7ac99b01c9ea432791495f35152d59fb22def5e109bfc415a87702e3bc2df337bcb" },
                { "is", "b95bc07f1f7d1e04dedb9a212fa321a1c82810b6d7e70645a9845ed54fea948911fd992a4bbea1d5d99ec4366a02eea2add76816a29a07bbd6ba4f56545426b9" },
                { "it", "ccd476f7d678ed8cf2c228c8d5eb796378af6f49271897de222ff6dd9fff39efb6e9afd0dbf705c8797e8fa244eb388ec3fecff409f85eb09108209a15fa8dfa" },
                { "ja", "bca5d45e91b951a3f95e63ef8d80efda2df9619ffbdbd8d78e06704435e8237fec0a9a0a54e438066c2ae811346466fc4efea6b276cd492dd8453825fd37ea28" },
                { "ka", "5a86a8179a5442906c4101004eb020a02dc385e54d0211d1a73c9206f8d0b0c8fca3706de400ea6cf0dd723c09c03ac0a09072542962b0ba653f98f3399dcc04" },
                { "kab", "d65bf9b63209c08cb08cdd8301c84c84823ce822169622ca7c7b5477732b446e0e4d26009d1a6bb847d512159d54a19f632c1cb620fe671f32615b8a11c7c8c0" },
                { "kk", "35d3c9b2632c1e77fae1f00d728bea9b861f4486025a5f49b4ba78dbc7abff72ab2b03e9c606d32f8f3bba1b5357d25627479a03507e2000b5bd26235cbdb7db" },
                { "ko", "e08fd59f462264d4b9a3f1e701d8801daf568ce23dda402a91a0a5ccc9fa7bc0c57fd2590c45d7dbb6dfacb4b4056f2e0b195432a8820e4f6b2d866ab71b610d" },
                { "lt", "43a8eb4d6fb717641e7954194f21315162a9d7454f9c175acef2277eb792f3c40972fae17932b1ec1d5b15f611977a203d6d4cab3b6c63e07393d83eb627ee0b" },
                { "lv", "93d6b6205dee03f9586198a4488e66f2100654626fce5ea4b41ee5922def7cfb678e23221bfd2f5d796cf63b2151ae7d20fa54e91496625b7ae3b908e38a21b5" },
                { "ms", "edbdc614912810ebf8dbdbd662a954f0efd7e696294314e3162adca2934675a72026bd66f32adcda035529ecdfa872b8f597a964fe0a6bc0e6b8f4dd5c2b0c53" },
                { "nb-NO", "8edba3407f818254d21134f84a8a58d1227dbc1a01ec25b88e48e09c38b0e16c236a191c4e14e71dc7e0b1423ae3f75a51c9974bf0f3a6a7c360dd7bc7b97d6d" },
                { "nl", "2543cc6d7b756f25372c22ad722ee5e88ab2b669d1b52d82bf116c6f1ce95ad2c73724efa20dcce90841a018711bfc8707c4a7b5d8743cbfbad3d29b4e690767" },
                { "nn-NO", "b7b375d5e86e06e6eecc1933b980c791cd1f6740a5ab3ee90ddf07b1def5b668ad532afec2d84df6e33a6c442980e918b6f94b3f0fadf7c4878d6d540d33383b" },
                { "pa-IN", "599f17e9973007b6bc3da73105e7ec00e6d72d61f25764b573afd6f610498d5b2170a18e1e00177747d183fbf1b2579e4c17c050a3d126bcc633da50132b96ae" },
                { "pl", "0121a52440d1a8bba2770bd1af71faad76da4a926fc15023a24621e921b8f05be3db85ec729e47d082ed123a47af069e13ed6488b757342a5e7e99021292a1cb" },
                { "pt-BR", "abed255f2774eb9425db62a1b4732f05cacbe08f5dcbbbb280b836569d246834aa4aadd0c95be728b0fef705f213dcabddfecd0baabdacf6865362cc6c09f96b" },
                { "pt-PT", "4bd63d0e2337d1f67b33a6e0a212f359d4a78ff461adf6f6eb08dcdbbe276e9fbfc61fec99e0c702f5116a2e1489deca63240293102d2d0dbae6c547b8a4aba6" },
                { "rm", "529694875db5d28a9c03c57a134363b8885adf8263ea1fe3a0a7672aad40de4a1f627d6eecace4dce6e911677bfc2be595c6813ad06b3b00397f70cbe5441f1b" },
                { "ro", "39af339b8ff47f95ce2c74c2a8f7facf47f544030b2195507c627bc78cc2a4de47ec4f53b5158967fe4fd472746f537590881691e12f3d145ccfb6767d3332f9" },
                { "ru", "f58f74b1fd7878fe34327325d8db375676e68b7ed4026e5ea45ad8349cae7234b1a8206abc61e38625770c07124330f398afbdd6fc3b859a180e320c2a4969fa" },
                { "sk", "fb43f4cd61ebbef460ce93e88d839e62119bfe85e282a0814a29886838f16e46b3787b1830a85fee44408ee7319229c4bc7f85656612979c8ba98b21fea6cdab" },
                { "sl", "55d82584806be801e4fd9055d9a3497305be608d59ef56b2ef86997661509030bbf9ffa45959dbcaa24efd8a4dde06f27382de4680576368b5dff7fd6a85c980" },
                { "sq", "e1efaa79644cce74284f814ae5552ba7269b20f9b251cd28e2bb032f4cf1ca092a5a4066f21587650f0eefedca2f87fc651a2742275584f2bfafa020a4dbef7c" },
                { "sr", "c947b6fb5d2febc90e31f72dd17b587fc223f020db9f77169051bd983f540c42dc617feb727688a79d782269c4e77fc32b6d34190f727b7516320810d3aecda2" },
                { "sv-SE", "99f2b5119f589edcb712a9436cb38112449827ceb3ecd61712b465847196264d593abac0e3b62157fa03cf0bca214e9bcd9b1fa67c3ee6f5ad9ee489d8ca839b" },
                { "th", "911e5f6d41b8297ac754bca77645d43f0a22002e60bfa61ad31cca1fb8109dad5d3d953bfb2f595688a20502ca24c95a380bfcecebda95d18264aec399baf094" },
                { "tr", "d5dd2ec91e7b510b94d4eab46ebbc2d9b2ffc504bf309c3b1512ddf0253879aa960bf2f49db96248e102dfe34b3750662786085b252b28d6e69f2bce2359f8d0" },
                { "uk", "561e051f6aefe2c542b84ff2ec6edc1903a03f988a01bb14e1050e3c4dd40ec7284f74b7fccd6bbb77b0644184b1b55529ce09703ba04426f18df87618c44ce3" },
                { "uz", "daff1dc51127f480ff68068b3c8ed23121f4760535e751134865b33165b0077a10c6d7bbfff41b5f3efa5ddf33fa2c383d13ab7b22bb47950c0b15a8dd54e2f6" },
                { "vi", "fea1e7782fecbcbd80685c1d3fcc40128c5fefbb543d64c7b47f62508cd89cabf9328d058c0dd4209405a945cc7c1cddf105756369594d0f0d3bc488daca866c" },
                { "zh-CN", "b7bdcd30975daf6997ccdfec5e02b7d21eb0bd74b2dbc279c9a3863faca1beb8089f045dfde8d5052f8f32eaaeacc31dfab26ad39780e8ce0b9adc0b279017bf" },
                { "zh-TW", "9404b2d074a3e41e240d37fe331645725776514eb86a22b9b83bc9f06785ca12107effad15d5f90c01cf79db98408e4c8cef7327d4563045ce7a2e6c1a1fe386" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.3.3";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
