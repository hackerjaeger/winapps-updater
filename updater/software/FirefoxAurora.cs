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
        private const string currentVersion = "119.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "0237595eda638eac63910f1d1b00812ca04f97ede83e2a4892ff2236437fe70adddc052eea81d840eb9d9c191beba5cf9759820607b07200387ecc533ba730c8" },
                { "af", "63661fd3b4853377088c146fc28614511772aef850a36c440d2c8266a092e50dcb1f1e669ab28b910aec945c3d5bcb46cb2c40dd592b9648d7cb664d60360a27" },
                { "an", "54d3ce4e26c72d614f673717c9e3d162798e9c51ebd78ef0f92bd4726d20e795afc31208e92d18bc990a93515cf9d3f0034bd98b711b963979513e361d653ab6" },
                { "ar", "c7ea643461b4d19f69b05cd5a3de043352caf66ebd66b7ccfac51c4a16369dacbae5e40042f4b5f6fae95bc3c3828354b732482a766783c53c74815c296fd86a" },
                { "ast", "c166a038f8f66f5af5ea4de371b62505099018bb9a63f2b9e59bae33f1c84509e2b7005d60378d57d57b2d4c97dbcf3975dc91f0824ccff30e9011ad3a391416" },
                { "az", "195df14bfd061626914d02f9b505c3d1be0f2ca7bbe926d55a4ee9c7c1fdb395ad77ad2c2e52b3411ee32a7016a5bbf4977a7a55741424eacd9e3b61a66af2c9" },
                { "be", "b8177ef3535c510f0f4fb86c42a092f7914d8de2aefa4daf6673b32fef306ba208e29ba5c2adfb0caa8b2f22eb50ff71cf8f0816775d9a5c0ce4e1413263678f" },
                { "bg", "54dd343be9b4d6bb437fb97dd37705e20984642afd281af80ce90d756c51e09344df2e61f45ddb753d41e7166d444e82a4fdbc654f4f9fb6bac1c3e24bc5b06f" },
                { "bn", "105339ead33a08a3152039fe0c89b4fd03ade987230f91dc6823ca7ac167fb95d3968206a0d6797c30bf407e0edcaaffe69f06459cf6154eac82b3d391d523d6" },
                { "br", "831b612310b1173e27ddac6f1b00144c53dec9e8f778decb3baa4c030c68755d61d4bfe2b9fca5129fa803680ea4169da8e13249f5a483c54a702226f3f7cbb9" },
                { "bs", "9cb53a13b940eb187bb3e8cd07af1af6378bdc962b96a14f8b41a90e7bdb9f91f615d7c9614c8a233638092e9f3518e7e2abf13547da571dbc9a82a2e622d523" },
                { "ca", "29a6a570d1f8013aa3c313e2e588d3f89b6a5d74b015bdff6131e75c921d8a8820b2038b38c36266a642fee2c87016daab9a096852ac18f470232834ae979371" },
                { "cak", "059ac666e23b755baf7e17971b0dc014aff59720514e36b1d394f82e17fb0dd019831346bc0fb44be3ed0a3c8fc17c38ba673c56e51f62390005a4e29658ac15" },
                { "cs", "1c37c88c3add90b567fc113aa01638d3386299a2740f27c90252114e4281e97e6ceccf49dfdb7a44958aa1264fc6af328ca28fd197870c5b04286572fa03ed16" },
                { "cy", "82eeb03fde267894baddc32db9e0530ec2cec81f6b5ac9c3759a22300a9a7583fda09ea17c2c8e8bf80d1029d015b590e02184488051bc965dca88d4b3ca4058" },
                { "da", "6ea5e18292dde7fa57278935e9cccd86930ce35de26c4346a095a60fe9bbec5a1aa323f268a3f9570a7ee10e4b933e548685bf1d13b15c881d56cc5be8415af9" },
                { "de", "3286c4a3c5890d34f9d714e26487d68c098b151f377525b0fa3e51a5f808aae8bf6a7a9cda53803f68f6f67339aee64b2337286454ef1743f71f8192b15e19b4" },
                { "dsb", "1c25b8600df43ad9dcfb0262973c222cab82c8af04cb1028c077fdc1fa38d8d7aa6bf3377c7479ce119b73f7447059a82cf3eb6df54a172bbb04b9ae50cb3dde" },
                { "el", "008390cc999c518f162242e44d7315f2dfdd19c0169164ff1076613b9088170467e48e242ce22246c1c0df1f0e35a1bf300a3a30480878b17744d9eb00f8594d" },
                { "en-CA", "23e68c1e1cbf91755a2f48b702dd66a474770619e586ae7151ae41303e38c9649110ab627eed2878955e2f1f53d5ae68dd505a0eede2333b1e06419bad343231" },
                { "en-GB", "669fc656fbda74532b4f1469e9b133e49ab6a1fb6d3f81a2b39de203b77d2f99e85e1c028ff3f926b9c5be0821f0ca7e3f1b993eab026d52c71cc5127797ba52" },
                { "en-US", "712891e448f94c9fbd9a6b9ba64f1d146a18b502df226b5f1934646e5db7e47176db85c97dd07214ecf5aa3bcee8775aae192c6e779bc909ec0b39f3652a35b5" },
                { "eo", "9dab51969ada4526a832b6ba11c6c056d206ee46c31ba6fae751cf116061913831d953de0070b5cb2b7bfc5875af7837d9d10ee4c8175268fff0ee13d7b42369" },
                { "es-AR", "5426e5b96e271bcad1f0331791f5e2de89a63ae6cc6c1a07422a4ebfadc1e70014fd0012813d8dd4015dc13d6169fe128b2acde4d017dfeed71a867748d09d3b" },
                { "es-CL", "64d9dc21ad5fe80961f1e4c768e68a80f8f4c876251aea53fc2b9203e4b2a4e067c226c34a200c17044a54cd7a5b68712fbfbb33929dd61b7094ab8f1a39a829" },
                { "es-ES", "457479cbf7be9eea2a44f01c8c479c5278624f7b6931a0d9b7aa8d1c3e668d3e1930304d79480ca6ba52a4db6bdeea90bb5f45b750a73101cf2f288d9ea2a4a0" },
                { "es-MX", "7da9a8bbdd30e076b0434affb550be45e422debe651f29fb31f31f1d55e52c6d415256f226c6734452c4aa3fb0295ce1247d08d949a855606852b22483fbb729" },
                { "et", "4cb1d2a131b7604a75f93aefd09bb68a7a5d800b52b825d6e587351968ac0ae407cc6167708668e8021482dea6008a4aeda2148307bb13ba4397effcaa330b54" },
                { "eu", "afc08c9bad9d14f45ab673e805dd099e75b12d9fcfa1840cf3c3989a2900c27e3665119b122d000a56415bcd67d3fa447540df3edd22c85865bc31329fb1dc3e" },
                { "fa", "f5e9e0b0306e1d1fb70cb2eea1361fc542d44948a52c2c71c4b5f019cc07bd6ab0a2057e3959e4e2bdc7bef13404357761ada2c5b7e5ff2e098e62162748f20d" },
                { "ff", "2b0316602fcba43ba21716e835c2dbd9a6ee3841aca9a3fb643dbff5c3161ddad49567278254f8a502eb4b1a38860d0196d64bfb9a83c7a4169ae6a3670c03c8" },
                { "fi", "39831073ddebd394928500689503dce1249e9cbbe257b76d7fe921469643db46461e732c744ed393d90ed6ec401a18e68a9cc3e5f9bf6dde4795504d4b458ad9" },
                { "fr", "1903521575049745c780967f0500bc5837434976878c95ed0f653309ee52b70d6cebbabed775ff4d4799973a77d1dcc5c1289fb9ca8e604963c7bd9b76c17afa" },
                { "fur", "1b7e884dc8c6c1b51b0ea5a8cde27a6d1cea96e4a1db04ec1489d30e27706121a5713ce1b0dc2e9b01d073eb4f0bb51cc8465b4b49566c51c767c638f501172c" },
                { "fy-NL", "607af4783121b00a6a12153835a04f883c3a8c0a464e1f07bf7b857fbcbd0ca7c3a81e7dee86cee07c0c3d44494ccd0b842b84483bd146a3fd8b875638c8a6d7" },
                { "ga-IE", "2dc37d137cd8bea5bc629c98995a56bad03b0cbbd5366da939b6b297f222ec6faef8b535782bb789afea77b8861a01f246af87bae59e303f08d68c259aa6f95e" },
                { "gd", "d7da9e839b6a151f7ba3df997e3aff6ab6d6a97980add2afc9ffc68ca7113f8be5913a611ea3074b4e530f943f4401bb28a12c0bdb8ef5825140b126109f4264" },
                { "gl", "667de11ad21aa9f3c58bd8eab216628fd064a5435dc74f2513a0550ba2d180fe43191981a5d147594640ecf828863d27fe84129360c4994d28ea826d9437c205" },
                { "gn", "d97f2ace219548a3c6b9964b6cfdee903ed618f0a0211ba366cac1f8593b58f93a5b9689a6e886066217e304a4a2a0563102f23aa19de22aec29d875caf97b4f" },
                { "gu-IN", "d0512f388784a3c1d0e928d74f1a50dc4b86883eb8b488dad1e6853cb4b70f91455b273904bc5a581f5b7f82c1e2e78444197c0576a5cf3ef81c416357632054" },
                { "he", "2c6533cb02e43d80207a66489eda481c703374d3e7d42c2d7ae0fe7a902ce71fc2d063349b1cfaf9f95ef6b957c79baf4af07aa287682cc73f7deeae0058542f" },
                { "hi-IN", "3ca35a75ce84f7697e1c8959addf24028e6d9194bffea5e64983a25dae9e20971145b9d51a61917acb42152d28a89d81aeb46e4b4d97617892cebca8429e7d73" },
                { "hr", "b0df608d2b065bea11920842ece891cf65991c852a74071b7bf354209d8fd9433474ba571229725b61fc6ec78195579ca0b5c27672942d2a097af3b0c61cf04b" },
                { "hsb", "5c96b22b9840a7e1938b7f3c948747bfab42e22218e2116742e38f6c277bbd0c0aade669c984bed610cfc866fee5f976866eaf01f1d722bda60e0acbbafd6b67" },
                { "hu", "934f67309e0640ee9e8995ea4a32eb9870e1409c048986ec503edb3be34874df7d6155c0e68af9b7616b83557962db17a65d4fb5cf1ba0f285f5dd2afde48645" },
                { "hy-AM", "645364e3fe929ecc614c10fc117b0298b7a16431f8b44371ffa04a3a529614bb7d0f629f0bcf6e577652efa0aa24e282e2114c1a0d921ce1a727e7b3c76ad6ae" },
                { "ia", "fbba55949b498b3a4277eb4bc6069f45fe1875296601941863e4e58f9d8ba23c3835804626da69f281f8c8008caba1234c31dc2927528fbc389397b7c21a13ca" },
                { "id", "d2cef176d5ae180a11273ee0a949e8fe55fa1fc68f4014b224751e2dd22db02ad7b6f7e9a65ec0430552a3f55828d50fcb8740ddd837218601e4f95f139f506f" },
                { "is", "9f89ec87d413efeebfadca1316498387c3dc936576e33024b783fb05c74726a65597eee84338bcc51a63bc8131289be2b2987c3117845b167428af6f5fa8bafe" },
                { "it", "950bcc25124800599b5ff258ce5989337699d70ac5650d55c429b1c4e16188bbf51cb92fed9015499ad4ede236f27d3134720afbf2f33168bf5afb15c4ca30f3" },
                { "ja", "f0bcf63717c8ecf86547d4a821facfe1ec86d4b65eb6a9940280374fc29c96118519b982d8d7c238125d6d0d2025c1b685f83da3a60cd43bdaff0325146660e3" },
                { "ka", "45e40e2d292e66b7a089862da07992e22cc153ee5268831c74cb2ee3f3f4eae33233e03c089a634a2338e99f1ac9db4d0349a26a9e950a65afda1d79a07983aa" },
                { "kab", "6a4a8d6020cd4af69fa2fa59fc8bdcc9c32d9a8b197a706e85b7c47e303f5b02dcbc61451b75874db4a687ea94a5b52658d09f63c65f8ea5850c638c5e3503fb" },
                { "kk", "dc0553cfacf417409a680a18b1610dbf2d0757c0a3529583c867b1f15d2833cf35acf9a6dc2b5691a0c1add20f88d218e7ef80d0d32e076c68d9e4e27881ac6f" },
                { "km", "58a4c7d5b87d3523054f468aac20a846a22d2e6a9aa64fca37b8f912276bee0268fd643dd6b0de5ea44e9100139eae45e3f0ede6df596d7a44b323fe26729094" },
                { "kn", "b249658716072c402e6628d3568f9649c57c53bb2b4820b74980b0273b0a64749572b93970d456d10ef831ed84488972c115a1c359e8506fde0623168c935dd3" },
                { "ko", "0cc686abe095dc5a3adaf565e9fea8f55e06addb5722fdf1997246bbdad9fa7859cd03032e249df73857efe7948e82600ed5975036e6360c3d2b2646ef05a266" },
                { "lij", "dedf767106e1ded278856d6b6052b856300a4ab6037dad2808efce092a702d364b432643a7b2f89f23129e8512b6d4c44fcc63075e85872b52513e02133bcbdf" },
                { "lt", "f042833f4535ebd1577182a2aa2d9dd4417e4dd6e1ef8f835ca799a0863735a384c78709bdce61884c465b26f9b267759edab8e35900c8d72d5f26c9822382f8" },
                { "lv", "d6c999feb72cd0a6b926e68a42cf889c17c10791f260aa05fd40f4cffe3c92fa9a0c73a9b9f4744d1fa012199b5abd6d5c4b39ffed16b044aacbe30d776415e3" },
                { "mk", "65728c11283c62c48039d8e079b43063f5d6f2eefc2f2dc944bd50a24886343a5c41546579cf553e1b1a593e9ddfce28aace4e04c61f683d859bf109c1d62f7b" },
                { "mr", "94e96700fcc939a0f2747279f5e795676d11a0785aa934c4de733ca0831e8d968115dd237ac9eaaef56c25391230e6984922bfe01e52d4bb3f91c7f57cbfdbd5" },
                { "ms", "68d67899c4704b6d15017300ecd17c1d616e1213a05191be7987b96bb013605136f017b82814bed6804ea50255260b0e9e1d6114ec782aa0bccaca6abbb0fb87" },
                { "my", "2b8667e874e48f544ad2ca51fa9c8e649792be05094281f05c9473ffbb2b3a89ee59a9f19e4527699dfab3e50335121b53fddb13a647df61174226c3c1b8c348" },
                { "nb-NO", "4faa8f20970b7f1e5abb674a35c0737af0b8f7b275adf700a30ea45dceb2fb31d835e8441377b3b637fb670b4e59f65d9e839d708f64cda27f8738e2463fb8e6" },
                { "ne-NP", "6f6d5b567dbab3751eb9cccc2d25d76420290eae4a1ea645e5f017bf50fe8ef1e93822944d982e2e78d366fdb665e01818061e6823108b4658d1f35e601f61fb" },
                { "nl", "5c9f625e89d909405bd26b2925b83627680596fdd8f54150661fba1b8fabfdcbe4e872d01c681ff665c5c7fa4be0514091891819b93e90258a98179bd68364fd" },
                { "nn-NO", "346968ab76c39be2583997b67e269ba1935646df1bbe7ad413806a034200c1ece13b0759b347342e5c1aab893c6652e2dce106d829437133a8fd61aa7d910710" },
                { "oc", "2ca87454eccc50795b3437edf702ae326b3e0996e1fd5f7d1c03fa3cd50db7a0da8feb05611829862410d7cabea47be8968a38e0bca141b910f09760246455f1" },
                { "pa-IN", "8f81f6e35233844709c54f0b3a4c0b6daa8028a97b3d792e9e98753f1963b3f93ec57eb937428b7aad5abf88bee3d7da7c22803d84c233bbda19934eb0238696" },
                { "pl", "7e3bedd3abef5a1ace6392ef47dc6b1e60e710c8f16c3b5b707ba0297d54cfa47e1bccb3d252dddf03011203f83b47e6860edcf1e9290eaf0937e080427b138f" },
                { "pt-BR", "46cb82368e75088fde43ef7678606e519cf9e40c206b84ede11990e3a84796aabf0894d091121c582597cd6cffac5ca23ee1b7489ca3e70355022682cfb12705" },
                { "pt-PT", "818f4b1bc75074d6fed1b0da963bfe4dc4ec31eb04fdc08b900d16a1b4c0d214e41e07383d45a133091412968822f2edd7c2fefd5f3d9e6709ae4de515d97663" },
                { "rm", "46df7bd4fdc2b604e595ff4c90d0ce818ded61edfb4e4d6c87f11954476ef0fb005ac8bdcc76394a9ea1898dfcc2663d7f0c7f23a649fc22f0811a7fa0e537ea" },
                { "ro", "abde1a3b8ff6cea9a01e8ee33e36965fc389cddb46e9aebb3d69c5e5ae27bc56e69e7ded864300f6ee9f05eb5fc8784c5baa5d290a5d2ce17ab1b205901fa355" },
                { "ru", "e8e4f9a251208d297df34b2db71424178244a7171b459dc4d60ba55350e05447f7bf5824f0b27dae5c26913de6f9613696f62c194a1b46f945248837a5e26f75" },
                { "sat", "68c0ff50d1b6242e00851fac9991e4eb3621890338876e287e6cc5f48b4a19e8bc4772fd15a4ff44bc7e65e5c8adfea8db7c19fd68a8b5bee7d2d04917ee0b8a" },
                { "sc", "d57449361cf1d9a3c28fa5b2d9e3d6ed330be7836aabd27d81fc3a4d94d7cd03b07dfe7a48bd6e2dfe0d312cd0d6d2876ae7ac861005d7fbe26dda5026576dcd" },
                { "sco", "f44b36beb2f707df13c40567ad9db2c0c3f886cb6903b13c56b5da0a23a3c1de7531e30731351ecd92aa59d5838b5a3f2f6a8e23de8c076d2f5c22faafbc3096" },
                { "si", "afeec68779b5148c49b320c2fafb069f1b43ac81a77e1b287d1d51537c7c49bf3ed260bae1927742e5ddfa45ccd82175c33fd148e52448a829c5117781f4947d" },
                { "sk", "726441880364a60194e420b30f1dc48b3a84ba243c483ee293784bd8ad303b480dfcdc92d4b5c32a252813c4964953c617bc1e28c583d5b64e86171c61ab49d9" },
                { "sl", "a3759317ffd942375d1b96200057970df529a1616dd1e913b6f78537f5fb172cfb1c7fa7ab6d7e903f3aba12677e0b1369eed5ee86b6dec6bb986b6bb57327e4" },
                { "son", "a628862c5802b6d03f1c28c18df4053acb1b8df3eb6324a96f97271f4a4352c80e7f4dcde689363e30c9bd69918d232bc4021d9ac43c2071128c0960e9e05907" },
                { "sq", "6c037883c514f89035f0532f89d67060edd23b38838ac0aad16c14538bce78da1ba45bbec0672588d8f04b5f33b7b8e73ca626be72d2da349a2093fb89ecc53d" },
                { "sr", "561f2e9d41e594807879e02d4ea58f083bcd8b92867aef898e21d36f96981fb087c421ad0949c461ba1c3d22aecdd589b58f0e49c932fa15b829fe95460963a3" },
                { "sv-SE", "0f17d4a87e7f19e2649413c83cec6e3ece8996e2c6c0e8363a8843a414a852e2e4acb81d5382aba258fb7806c7d45f1aa1e65960f506af848e8378d3cc59a3ac" },
                { "szl", "de4df8f1846cbc342aba41b68c208f855c1bcfb91a65875e2eca9ba6e728e05dff2a1d82dd8bbbc252cf891cc1038fb253ba8cc42c0b0e52bdcf68646c10582a" },
                { "ta", "c3f5a68efbfb60448ae3121bcddb22ad6e4c4a0ece758a7c30c9a22d6a7dabd9be091f4c56ba9ebaf3ea7741a266142746369c92b5c1d9381c9ab8383eab4a5c" },
                { "te", "bad4ba2057a3b883938de5b11518a40cc83dc789bbc9e87fc53204b428db03356e2d774f7288fe06b59846ab86e9944db930df8b806336466ee7595578e731b3" },
                { "tg", "7ae1296e2237f4cc58d7572f1383a5d37b65348ba37c70a03e27f9cb4d93815456fc4804df14205a6bacd1f12dd44633d6072a305bc1017c52175223077be3af" },
                { "th", "4701f41a2d65b1ebe44d6a504f0d25bf419ffa24b6f163ea452a5c6eb98e52600f3e84ad41d1bbf4ec00efb5cf22fad1e3999123226fc594293edcfc2f3a4cce" },
                { "tl", "1d33803c322539c15bcc874087cdb0873550e0ed907548b549b96af52a0e4e8111a0315084f92fb581d153190c8ef22732d869bc05822081da7c933c75b47d6c" },
                { "tr", "21203c2db0231ff99ba23b08c09aad571069c8bedad860bd0362d9c9b079f91da86cd86f9f9926f0dd9da21724831331c6535f9356cae02e32e171fba56e8b60" },
                { "trs", "0a6c6392ec93b03adfe7d1fe3669e381a96ae0a503b9174db33988137df0b60ae376212f8afadfd818958aff834303c15e1ea1dab0ea5065921cdf7c4c7d471a" },
                { "uk", "e2b49c328409442e1a05b170472f22cb82351d943c6ef297b287698e79ed04a75c2d86bc2927a8e58ac7db27e93d4644263e1925520b3d77578dbbb49778467c" },
                { "ur", "c30ccb34dec0e7da30fa6b9b2a7e14dbb9935df93f4434783c1de0ed59c6f40728f825511712644ed50d0042f09dab968b4b252726d28e34a7d70d36d22256be" },
                { "uz", "e54f8c8bda611af3b3ac136b38cdeb883fa69374aa3e5099dbac0a6bbd229bcc2fe45002eb71a518e560111881f14c3954d884c56774b7dedc77637490e05a76" },
                { "vi", "705f6a70f1ddb93dadd63a6583838b6e4b64a3463f968dcfadf1c0e93b11d303ac34d5b7bb255492a5529ecf056b870b7ea1b68d1973e0a081d49f3789ad449a" },
                { "xh", "1a722844cd5a0aac641758c9dba29f4ce25cee4c693564b0f523d665f7914c2c11a0eeec552b5a64eff2f072a2b933c557a9a7435d30f6a30d1126c73f4c656b" },
                { "zh-CN", "7d1cfbae2d2dd9b2916c4504cb0638789366c185063555d125804fe9439042e0a90bcceeb0d2690fcaebc626da58161c4ec5f41a2428f67ab1eb8bb1435c5e79" },
                { "zh-TW", "a69bac587fe5e93505b46b5452ab7dd8cecf2e77a449d414f57b65949e4c809e65c4e8ce6277518d611fd4401a51443e5e482a44b035b5f6d699ab0ab9beaeaa" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a5c6a6a5e3b489c16fc7cb464911ed8d06869196dc6650089b35e20625a5162e50bc33ba2f36c25b468117b0cb5e3e5b1127d093cedaf661b8c44175a46a97d9" },
                { "af", "481f8630b260509b2b87ec3524e848d965df54f49b413770aeb99a281c3520d2056819bf45be55c90edad49b89a37042acee0c9cdcd065e355a010b5f9128b49" },
                { "an", "5e30e9bfcf24502614b683705f529f46b1369ded0272dc676c53d383862f6a6ccecdf5bd32c085cbbc967c96c5e664a01e536e5e1ab03db4870c579288e685f6" },
                { "ar", "a4f272eecb18d7fdebf4fc203483ef846f1a698327d269a3d0e2bc19fb9c54fdedaf44384f5ddccaf593a1a649b5dff82b5ea00ab983a434b2ca58af22d86b85" },
                { "ast", "307c62d34bd992d04661483d47f9fb1fb665e1bf69b12c21191398fe9459b0bac525f403ef7424f90a9517c045eb41a03ea3b9cc429c922404d58e5b6643747e" },
                { "az", "75febe22bf41ba2b83de7d37aeb07c98238bb31879bd849f3f05055ae420408736b20fa753899f18f41d15345240e733c6e855ca7fc6e9088b02077644ca578e" },
                { "be", "1e9b4bf8020fc8338c1defd8168206e8764ed5cdc29e0cbe5be540adc1409d31119916e69a629ba29ab8f1a32588c626faed943106216f1c66886a3144b5d96b" },
                { "bg", "ab9f28c7bbaf3a7f702bc35f3e40d70f7d14ae12e2cfa025990ea260771adafecd4e8229b100e0bfd750f14ddbea0829ee27cb50c9b0cd4a1d4bf3d01b2d4a92" },
                { "bn", "0c534d2124f6ac40c7330483d5b56e880de5f70bf37aaccff4007a1707c68f45e1725d9c67aa80857b93a3e09872b4b0384eec2b51620d4aae173af93f16b51e" },
                { "br", "e92e636d1888a7c3b93da8bbe95ee80e296be2763cfec05b869cd8f740219e73126fd022bfb14e2b323ee2284d372abd1d5e57ec557e697e0781ced5e811e174" },
                { "bs", "aeb890112e378bc06c0f0105534f822d4caa96e782b75ba666d20ef9052163662f435746de21126246b4a5fc2038da6b9e90d7d37b1e85a3e07111fec385177d" },
                { "ca", "99503852aab61a207cdd825a94f4638eb785552d119f15f057ef2d5562030d420dd4613a5795759f62b84cb4bd399814949fca4c73b41113a3a48f11debc677e" },
                { "cak", "302a20bf704ddfaf3e963a1a99b65f68aa1799021489b9978d4cc3549184d9b2bbbb0e2d5e224313cdec4313f92f39874dd129772f9309b2f041f6def440e6b6" },
                { "cs", "7071f10b02c82f1ebbc00cc55dee74bd9574def60e8009390dc12f877f91868fce2405c7e04a439bf48020af23626468c5d908117b8973b43d1dc64e6eabfc94" },
                { "cy", "0f47fac68d2dfaf0971ca67be6d19740304bcb0c86dd418ac19f9f6ff2e90b9ea9ec6beb44c0c7284a04dd41a427b2a1768ab733aa695bc9a63749189215130f" },
                { "da", "d20d96e7b0b541faceab39065b4b094979aefd97e30369d77f6ece15772d1c275388251f4097970851f97b5740af8b35f94a85e1dfb83b02499a453182f5aff0" },
                { "de", "75db21e0cec9d0253ab642cca682d7ab1eaf4330796d59f0a0b416f7193aad697290862cee4ebaa2593975b07848bfb9ffaf64746049dbb368929054791df4f6" },
                { "dsb", "f35284097bb1e12e4b83bd2ec666b8e89c7089ea034be4109e2ffcf5050fb8ffd281464fee69b4c6e2f4673cb3207ea9af427c94da2b6fe0360029421399fcec" },
                { "el", "92a71644e0bc24bcd69814e6280274e9e251ece186ef5dd2ec6d772eac0b88af36307ebbd292a14779968e805a5050347811388a12854f58af3893f3ccbdc9f4" },
                { "en-CA", "d7cb6721e6d90b45688a8bf53edfcfc758a4236a0f29299de9b4fdffa3b3cfa9779f315f80170846fe3cfbe7f98f5577eeb0da6c5f140c70684610adb8c02e19" },
                { "en-GB", "5ec20ea124f4114e35fb5f0b218a979c819662bd83521671f68e7eec696dcd0dd7bf9316e007604b87dee4d81556adab1a458fe034344039a17c0a22348e6c05" },
                { "en-US", "7179a54104f1472d117000031ef6277edaad24fadb18fb58e3eb659ed230b0f88ba324ed1045b160a094562e246d1c09a7d34c85fcc656254c596957061fb43a" },
                { "eo", "9e6effe1cb655b9001d5904ff44936829a30be28773bbc124cf5527b45217a5efa2e4a951abcbeb6555109e6269ebfba67e3d19738b34a93ad88b348e9ac514a" },
                { "es-AR", "fee5aeca5ba349e5aae8e5613cb2e9a2c91d8ebd6f727245ed384b69f2484eaccac03167735a15d70ddee932b2be50c21c7d237114b5e782ba40a7c87488c5dd" },
                { "es-CL", "6e4a28a4bf310a06682a11bd8031f5dfe6f39b95c6500e64b4403c7e54f3778689bb4ed39adbade7572f5aa5eab68f9c5df1e84a955b973bf4d1652be730590a" },
                { "es-ES", "3f533e9a28340f43a5efe46d012dd17113721c457d180d662053f1058df96fcadee956a952c0ed826657636f2741e8ac47dec563a8fc66e334269e093822be31" },
                { "es-MX", "4366a2e37274d53d8618eb53e3281c335417d24b382e380ca9640649e1ccd4cf254b3f3e9faa444d2e84419ee5cca3dd2466dd7c4f53ba00b3551e56d0a6aa71" },
                { "et", "e85c39bcadc3b6ea292a7fce9d0102276f0bcf9cfb2e2c00884d2ed8315b382517e70ea2b534ff31c7a75d742c105c46137f72606e20e5419ccffde1c01b433c" },
                { "eu", "cbd50cb4cca2849a95ec747d84ae858ba5f3a406dd6636d2350383b89fbad1d85a732add1dc208df9c18f2663757501f7ca7f44f2d1753397679089a5cc2332a" },
                { "fa", "1a04c6c382ced50a9e5e2e08a400f4560d995b52a6d949d5db547939ba8a645632f010b5b42ccaa00bcb163359b2c2e88660919d9145ac3722f19a956ce9fcef" },
                { "ff", "391de2a0db0e48dcf0248a330645976467bfaed8fbc80f69440cc703b683e99b0153e3e3ca018eced11b30ac9bf39b3bf52dbe626313f0f60070e99138497995" },
                { "fi", "2d234aa75144b3a06838cb869c66e6486a5ca4a80dc46c55ea7264ab2205cbe882cfc355955d6cde9448cbd4714d9ddf98d840d0e17baf75e818f93010e3679c" },
                { "fr", "47c892633c0277b23f4c934c36bf2adaef0c47931bbb9a0a2588c9087b29755513e4201a581cdc638bc36174f4348456f28f3a66a811b16ed648b9d49eb31010" },
                { "fur", "3bbd83233367ac12202147f77676d68eaa392d3be2987d5c783e502a8d5ac37fdefcec38d7f109dea6658d72ca81e5a66aba943669d49b2897d07aae43d35f81" },
                { "fy-NL", "924dff2e770af765ab357a5bceae9df1042320cff1b8e6540af96db5a9eeb59ec0860976a7e776238d011b2db5f2419d85f85a556b977d32b97572feb0003dff" },
                { "ga-IE", "f9209c3a8a9622ff2047eab2d71a92f38b6270c9f3e3fe30b57e06dc07648ed6b71c018456cb3fec14057337e9e3b9eb195f8697140170e526de52218d3592c6" },
                { "gd", "b11fe57e2a4bcba85461a9668e1c093f829023039edf2d2ba8a32b6e13a911bbb5a1e270d57b315e61f392856186d445b46a439cec58b79334a5b2909c9bacd1" },
                { "gl", "785265e2639142213d6ed54af3b63fda213cb0e587af059a145e995f09f4d178c7f1eddf1dded2e3e38d2a784054e7aa43e797466a91d4706ad1f38b6f2ebb42" },
                { "gn", "b9e90c1268f521ce12bfcd0fd2d967b441f619401c8b5b0da7647c835400b767dd67d239e9db63217835836eeff2116a6af7bbd72f4a247ee1a108a631f2caa0" },
                { "gu-IN", "81802bc043373c0131638431a6d5ccb92e24b405e5ef327a800518c7dba7f9a8801fe5d961e119537ab8469a03dde019f74442e098ca45fe3c5f1b77d2f745e6" },
                { "he", "1a83dd6efa1ad5ad20adaed3262ca09688c8a0fd0a53c0f291db7f8325c9dbb7d99939ce80049d24b9d2b26d4ad92efa8952abee4471dcec9163b558b601bab4" },
                { "hi-IN", "cacd4a9964e3acc2167e88f627cd4e8a96b4dd30b0d05ced176f1ed58d813097fdc5f844d6b2afe636fc8cb151bee9cfe2ae51d23a0df0a6be4e655a266f86be" },
                { "hr", "68c0185d6dc62247b27714ee28dc51b255db49f07b4a4bac370b3e43b0fcf6e63e47ccfb57e380046d9288a2a5f6af45770f3640658ba785ad6984ea354cc39c" },
                { "hsb", "7d2c87f078cf46901867617739023ef36320a3299278c9916678c3699ce8d21bace006b6723f6f384c610bed15bc432d95c73b3f61a5f05c2bfcde562c2b1e40" },
                { "hu", "9ebd1011de70fe23e41cca2a02ae0dee6cf36d13e6e91c520d56cfd22b92300e380a8b3603c7bcf215a68f4353f587a52a80f9b1c41adaa3a022204033a8a614" },
                { "hy-AM", "9d33f922e7f96342f69a14987cbff64556fe74b6feabff01d44e92226c4f3c470a2d92ff524161cf76c9ed792964d4fcd6d3a6386ab8f815ab0f832de6ab044a" },
                { "ia", "d2645c8c557d56a310973446e548d1edfd4f27a3c0f6cea40800857114ef0236c1089b022dbb2c44b76613261cbbc64c005f4a16977dea90c411734caa368a23" },
                { "id", "a606008ea3daa8da0d14e4a1dd3896919c0e0e71f19350496e1a7fd129612464e94aa3b288ebf270ac30fe1c96a3896246048b931a9ef9dce90ce9875e9f1a1b" },
                { "is", "e7a6c017345d35c3f99296e0fe6ea485366f8fffde8f7741d17b863e347102880430eb6dca800202e4b6271369c02a22263622cfe659138f19a1e849d4c5ba80" },
                { "it", "9924d2769ac0383693338233c10c01c21fb8c7cf99a493333be00c3a9e61a2a57b126d152e1b138a1d0fd501d79c28e817ea3d1e71bad2e7e6bd6fec43978212" },
                { "ja", "12f3f0fc7de882a96eecf70d3ac4d4fa59527c398362474b25289e6bec43e0b1f97494ce97cc84003a8170bc8c34f39509a44276b9db73b42ef26f1e462549eb" },
                { "ka", "c485a4035bec0949641e1570f1ecaacc98fd4365fac806a47569ff780e4e24080aaef5080f3902c1a5c895750739f34a1a93eeb87ecf320be4405971e610ce95" },
                { "kab", "cc29e334e973fe9f12e1619b6980e1923c7c8027011317ead58290a706f504173843b387ce572f7ed621bed75438d37313e7a7b42e6dca82af349ab78c6e8fe2" },
                { "kk", "a92d67aebd300df617139b04c2a5b88597dae45bbb5651120c8a80b4949842eba39071ba512123efd10aa87e0b1ea902bcab2c79aa51099a28f01903fa344907" },
                { "km", "6322b6bfb840f63cc400d8d900f8e9e5783e5c760cde32c5b3c12528e4d9a12205a338b1eadd35e6ac2c05ed542fe3dd11c6db425c61c257585758a61a8297c0" },
                { "kn", "7413f20f80fc7fde208093934aa0f5444d2f63a28354ea82e57fa5d5d740c1b3903f7a00fc6a2f6effed9fde74f922d73e2204e8cf090c22ca6340d95ea960b1" },
                { "ko", "09259e64e308dcbbbfca457e8ba31371d4ef12902d250b4744d0abc37d073303e4eca76d207f4420b0563a014cd73a5cff14a1e958f844aa0d7a6a9c69ab033e" },
                { "lij", "c4c082f9375cbbda0069d341fb530bb998c6ee99b228247cb4b85b8d3a2fb0f340ec0170b9e945cee69134f1303db629dccc4652878a9a48b79d182848747306" },
                { "lt", "d6fb06183cf88a08fc798bedc3599069949a8d656a3ae030c74e039f7436ab38aa3c57a821f74247eff3d03c288e75864f89398a1a3a2c36f6cef814c1c9574a" },
                { "lv", "f345ccf3e255234684f095048c21c17319fa82eb99098540239b5618f5bd2ac9b2cd46f795ec74aa3ffe67cbfb779eba0f4f2cb1d1a2a4f847fe0bfe2ea12cb1" },
                { "mk", "38557355e8a91327af179025e38f48d3ca51ea57b0ce8111a97d0b00b7fecfecf0659d8803361b935ea57f54cb53385d1b301c70fa7db9ac29c179a76904504b" },
                { "mr", "a7c3d36e84f05dc887dd273a84caeccb6128ee004bca97aea3b93add23dbedfe78c8606e937cee727ab97a78ee6675dc88eac8e14bd0c0172a6ab7731f005ac8" },
                { "ms", "a1c32bbf1b7b32e89a138979088013304a5452b2522b6300a734db3a7b813fbbdfe35954b6e3de4ade2ac1d2f6b2d662aad6c1ce5da79b2248234cff6ed66c40" },
                { "my", "f583976561ae81766be76107891b501b483e7d8b874e40ecd65b033946885571c7ef287ada697db26013852219e23e989afa6bed13f0ebe80a6b0f51ab1edac9" },
                { "nb-NO", "589b73d1c25d029c7a9a3000fe80bd974ddd280c2ad01c970d717a113674e21c81529d9b4addd2ff1d4481a074d03824255a78bf5872ff9484d28736a6f24d34" },
                { "ne-NP", "8fc1214641caea654c7157ea837bd119d632524c393a950339410f973925d29246e53ffced42d60f94257ec7344cffa04f75bf45d64cf63e5f2b265b06fdc228" },
                { "nl", "f651f10953e79fdd0b8f70cd26aeff7011b7c75f68967164f5928b4d09ded8343935714f8a5827f34d3aa92d6d58d9ce4e852c2387af5b77f40836221d07e0f0" },
                { "nn-NO", "ca7b8a2168b613c0165fb9c7c96607423e4dc461adb5806123f075d15c2d1890f9be61fb9cec066d6caa281cc556ca2c73fcdbcd476758c4d45eb0edbbe1cc22" },
                { "oc", "3ea41bda0bf30e2e48be48d0230111b5fdb5b1e84abd59760c446756ada737ee205221a3ebb76d52ac6ccf47c1481d186b2850cd6f19f81b27753d80b959a431" },
                { "pa-IN", "a12641bcfa106987ac0d4f1f801cfb27bc754143cabc0ed7bc95a50061b2069c42f2031d8d347421d885dfe476d94dfe6f6751e9348d872a7a3307b97b68ab06" },
                { "pl", "53a05a500c2e6f1fd4ee278e2051db1df4edae99edade61f729162881b87476d68cb39098af7a20059c6476ca505c6076d9deff3e253f34c6b564415950aef11" },
                { "pt-BR", "2a14344a2e2465443893bcb21fc9608e2367d273dfcf7441753be9f744736a91e79fd9c123b3e8573196e250b6953bc7e351c0dfded3cf2cb9d1fc186ae883de" },
                { "pt-PT", "6a11711865342d8f5cde78d9034886d29f8237d822dcfb4b615a77aefca4f2189bfbea12268c275c83b4f55e95820b273403c0fc85acb53d97a6a75aed745389" },
                { "rm", "aefac2a5eed99138d8be7b05aa084664a7b050e5e3551987b7b6b370bedfdbe7c9b37d582eab7728719aa724e58255b5b1993f39140ee9d92781798106812d95" },
                { "ro", "112479a9e68806c31cdaf962aebef3fe04f3ad1438cdabe815dc30d1913291588d6917908e3cf958aefe84681317cd68fffe6037c6b4808c98082524caf7f895" },
                { "ru", "6f71b9a171c2848f5c1ab7220dd0d618e864e7b465f5be511bd57faeb3e65bee068dbda693d5c9e151e6816e405d733d70af3f668c49f8096fead71382aa1e90" },
                { "sat", "5f61af2de800c5976e4b86a57ee32ff940a9d04911ee4af58f853cd76ced9d9d77d57ed8be2e30a622ca47d45fe07383499a3a7fc70c7f7d14f27d48b0d09c02" },
                { "sc", "b32193ff57a746da0e7e773c22b40902a957ead4f892cc660a8f273ca9410430ec3409a60e39ca3ba7bc902587b17154ede477da0d25f931def4247af321ff46" },
                { "sco", "67f04a6e6aace9604c500011b2c652f7be14127388549b21f16785b7665d045d84bceaa8ec7c8a78ebd32678f619cbc9650b46257bf40a059a5f9fd51a3b12af" },
                { "si", "4a0a60527000e668fef72841e3cbce0128b10e02eceac77b54010e890e2a6ddb16ea93b5c0c418e3c48592259554e41901e25969329f433eff9ca393d80ec3cc" },
                { "sk", "88e8c638acb30c8f7298e54bb84110415bf2f6eac89c08f66ae8cfb6a131a9332235d49cac0617bcc9a4439c0e824cdc9de35d1215d9f49ad910fa855aa60511" },
                { "sl", "3a581e7c8011dc03111829482c2a45039f6ba04bf5b04dd29fcd3d962d54254d35087480487ab6235d570ce81b9ad8bc8b1c341941741cad34df3d93a740c44a" },
                { "son", "9f61292bd6c4a469815c1e61fd312fb25b775ccc19c0545c8b92f108b95d265411b095dc08f8779cd2f97cbde4f72fb6774b2560b74480eb0ecc121d19a6652a" },
                { "sq", "fae62da5989f218497113dd99e0c396be3aa129be767baa44a65759b222c848ac777fce2d1384fdcd03f858399781e01c6c89696d1cb75fcd76cb8a3ac38c455" },
                { "sr", "9732d3bd6b33105dd2c9d6dcd0454c3696c3e090dcec01226b813577fbf5502662860cce1c5ac05a8067158d88ae458293796a00399cb193c94d6e6839c04676" },
                { "sv-SE", "75a4901c85974e4b1cb2f51e0443da969c97b2b29997776bd4422994ca2c6eea1f4f1c930e30cd1da513bf3f01da78b59d04214348c6caa369e56ef60cd62c39" },
                { "szl", "5936df147c248ab452899281be3d73b61c6dddcf4c706ff9caa445431451d50bc1b4b52a7ea94e0a636894ac9d2a4d53a1bf0302221e7123acf4c6a7a8e55b7c" },
                { "ta", "f54a6e9fa1e8cbf0bc37166ef0872b2ef01cd7d1458cd7cb74309b1899450fbd1a584b0b2b92f8b91b71dce0c11565f1f001ed58cada0e2ca4cfcd61db225143" },
                { "te", "077e29f834960769364d0d951b2a9289e52e79037cad7b8772dbc90ba1b5611317c6dbeab782b537499cf5370a19940ca3b6a6ae12b8a8b2beeb4b6b4750105d" },
                { "tg", "37353112d0fa17996a5e6fcda3429ed1ce6e548e8af4d2b0cd66d5c302b81b5d57377a3ef8a9f3f74c910c5722a7d0a6bb589cd28b4aebc0b798a23a771ac516" },
                { "th", "c0fd71cfc85eddcdcdb433d4bbc1506230fcfbc528a49dbacf9a721276804e9282d313a608aea61caaea9980e7c1055cbbd62855d50d50a35b226719968cf1c4" },
                { "tl", "3991de03a1406840168410fa1172fd590723d4c64e647feb63eb9a89963044c128640d13c89636e571da0105cca87ccfc7952f615d5203f6cb5ac0833e910cd1" },
                { "tr", "c28d88c45a31815f95ce0a324ad478b48d1d19f371c141aa0ca79d27d5332e7a4563dcc5688e6b01846ed8e0e8528d2c1279da6ee69fa6e8e0a1f7facacb8abc" },
                { "trs", "bf3c2bc404c5ca374a1c3d6b1ca2e4029cf348369712ba30719df93577071eb0e591e01b5c95069db953c67c099d32264fbea72db2083c7844d9e3e2a966409a" },
                { "uk", "6c1cb549251abc00a61ba572eae70343e8b9a6e8ad1efe6f7b3c5ff91a5d51506ab321c2a8a8c807202473c095f82816a1cf147fdd918d7d78b5658b9606a1b6" },
                { "ur", "27e53ef1034d9e75452a0461b1f1df2059fbe8b2c991f8d4b1570d6b54790fee60ca4f7328f72f16a49c4559e669ec84e26f5384ec46a64e80186696c84eb35e" },
                { "uz", "49c62624eebf6976a7fd830cd077b306dc5e873eb567b4d242170f8ea9f18eb100dfb455b5169b47666c3dda74dcb6e8efdc546720f6892f7022dbebd0c78307" },
                { "vi", "de064b69152bd701f3780b4cd89df17cb931b4c0449b04d62f29b8cc810821ba41b9b60bcac52806d64760d552b22cafd6455f9d102242b003ffea0a02fbf4de" },
                { "xh", "c0afdc9b83cd745f240a77d3e3e738955a1fc3014750995ca8a8f63090e8dd170df8987e3eb4733f8bc3ccbd468afbf333d3bc08107973b68bf7a762a2b583ea" },
                { "zh-CN", "8f63d771b7079edb4821b8cd7a0e8bfaa43bfde97298e777c949c50d1f320fd0b034b5eadc1acceca7b657a55ac91168bb64c02907422cfa00f3078d1ab6fb59" },
                { "zh-TW", "121c024d31f6956cd37b92f445d818a3788a7b49525a4d617ac5f36f52071d75357ca8bca378ccf1f007d54dc3ea42f67d54a1bc84316032c6c47ffbb99cda63" }
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
