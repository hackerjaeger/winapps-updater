﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "136.0b2";


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
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b840d6711876bee0ce742c4bf48651bad4d82445be41a2b9fd88320944d7f4753bbbea7cc27cabb96c69155c202d6ebd21464e7d8586bc9cd27363b38a05cbf5" },
                { "af", "1f8c12b770d60ed4714fff722adcaf6851f6fa0265eb6b22b1bc16011218b0040b95a3a1c50360a2e8c786184273873b81b8b4d726d390d737fc278e1ffa6a90" },
                { "an", "8efa1f8ca3b95355ae6b4eb75bb4515a4438b2e995c98b8f00084b0007fe8102891d465897904be7d82fc215a4c1b62b90415c80353a0aff6eef7b20f9d2f54d" },
                { "ar", "dffceda686eb97157681045da44a804068c60a2f73db6c2862c7636fa04e29f5a2bf725a207a0ed246ac42a29b6305f03c0e1dae6f5f5a96812ff9f311a27a0b" },
                { "ast", "411b65a95075099228c5909f82ef3208a078cc555cca816c803973d2cfc9a2568318bb8a8768238de9d339e4d9782b23d7c804b46b60b77814515f921ebde277" },
                { "az", "49ae7d8457b5f397bae79dc64a2b94e9058fde6b218bc75855c8a32e1aaa08da109d0e4911f1d658d046e53034668751a8c505bc0b6b554b32eff4485b8ecca6" },
                { "be", "fc3080bd7c7cbb518681acf4e1b1bbcb5aec05797e01833c4b8dcae44e60100b8b30badcc681526c93b3295e752219bbc2947f62dd687780212df3f0895fa7b0" },
                { "bg", "7a9bd1c2303ee977b11d328270aefcebe987ab45175ecc7e41382aac4a92fca7f98fbc7bd7a138c57ed3f960074eb9d00188313200f0ffbe0c65e8b9d14d0a8a" },
                { "bn", "28309214640c81b3b08b7d2a566f46f4fb6acee4566c07bae877e11b4c6550cf686c0154a70c5607320cd20fd392d47293c16e58be857f7db877ebd41efb27f5" },
                { "br", "b61044b12c1f3e3a573cc70c33f6cffb1c9a350242960144f673e07481b1fc7aea14fc3f6cacda3a799d7bab2fa644f70ca9259959ae6cb8a0606602f2351a23" },
                { "bs", "3045a95b386f9d30c028ed8c52b05aef47c60b7bd394b8a8052e841e01df2f51f3bb1d13b328a2692406569b053f6298b8fce07126781a82eedd025b8efb626a" },
                { "ca", "86ef50fa117c8e3cf88581975d11d5210036d4d27aac674b064f337a6f1239007a0b565fb041de3b157f689094d56d07c571ee6e0d742214c0f4b691eae02f84" },
                { "cak", "af66c76b191573d89bbd8da5b8c88bef54d989c592447a6317ad8905cfb14de2256858ddb00a873ac5418804b325a4abf84cb392061f97667e386c9d6b2ee1c6" },
                { "cs", "848549ce7ec3c3e0f82dfd1c0cb8bb1da550863dab263829a8aabc3c60ab4eaf6f587110ae57751d4e5a94e473acf09f527bc3c006ae3ab7ae892bd191e98383" },
                { "cy", "f65dbaa34b6e6ae1324796047a06c1efd26c0d34376235a209510940a91721a39b480fa18de35f5db9508d3f9d568e45eab67237b05c00a1185c096aea0a716b" },
                { "da", "ea8f0dd8665f179f7d2980a3edfbc9bf1c72126298bc9a480c3f9939e688e4a3af6b4792722d11ac653b89599aebb8526616c025557fbe6ddff26b3c1023f94f" },
                { "de", "d8407f669d3c1e763113fa1912fa849cc570af8b89c94d1659b6e23d45605f935e295fcacd6703b7af0bf88b1d830f45f5feffb66a2f08620967ef3696baeeb5" },
                { "dsb", "af37a1013e22710a2672e677a43f6c3f312055288641a3b9f700c963efe333852ab7d0bf8068590533ee0c031216abbe7c18a5b23bce16205593e03b7dcef096" },
                { "el", "6650e84e8bbe295af1f2297d9a6aff12c7248e4db8e816dafe02e9cd5d3fc24976e4f658bb499e64bf152625a9e18787a772976dbbecf50c9bd5e826d1b7046d" },
                { "en-CA", "81d95178d357e514a213a5836562afba1d3f4beb4ad7db36a135db31a5e37bd7db7518b3405b4d2dc7aeda7d3148c09b90ed317785ef2008d4d87be3852ae697" },
                { "en-GB", "1901ebd874befcc1546adeef1648477b9946300d64517ca36b49eb2781610270061bbe2dba6cb4581065e45f9e6c1356d1ce2668d1e0085bd8f63f742ee096f9" },
                { "en-US", "cdd99785ce1fb50ec669104cf7159dbf9b021e4f2191a11a084a7bf0767bef8a5efed3a89e21a0cf42fd90373c87052a2902d7b4dd27ddd0b448de6a314d8273" },
                { "eo", "54f953e87f4b92591a308e78e0213bfeb78278d98549231589cbb4c3aa2a37fab6b8c9777c24ce6e5fffa13694f7b589b1da8d7526b28cf9db27bb51f6890f60" },
                { "es-AR", "82a31b8b37a39b18e9f0fa1a972fb52872437b49cccee626378fe709b87f4496fcc16d551edd288c0d84e4540a25a5f1d1831f19ed5b40ec708ae722a1925191" },
                { "es-CL", "8b7902de779f9a46679a7f23d1bdbb2a078ea97b67142e979a1867a76bbdacccce1fd7fe56d78cf77ea3a0ebc0923df432dbab02e78c83757272ecf58addf814" },
                { "es-ES", "60bd5988171eb25d305c0aefc909d9b09dad5432b9aff032d3d1350bfd82374e3817eb40f6bf1e01c7f28f2f7aa960a798f57f6f49c88de38e32374004526b19" },
                { "es-MX", "3b450e59c34c9859246528dc0fb693235e8c35648476f5791115d78c86f5458b1bd19032a5b95bc2265987012c5e8a68e9faef9301c8f755f4e772a6124891f9" },
                { "et", "6aeb58e1b91c2fd511ab5b303c054f5cd96eb6bb0a78d3a82391bc809f61620f54052714595e959c7e9379dfe2acac4573e59035b857f33032b9f3a316f820c1" },
                { "eu", "d929e7e76dea84ee024cc20a2cae2729200a84fd20c7b0fdbfb047c82c986227d063c4d782036a670ebbd63787f624ed188d16bf403bfc78dc1d985a84114074" },
                { "fa", "ff2eb28968d900fdbe4baf450073ccedc7956eda0fcff257ab33dfd064d1e74cb2ef39a97974a176fdbc71384ac37695d508deecb5285fb87daa977a8f1d17bb" },
                { "ff", "a4071614cd7212489bd9ee2660007225120bee9f5aae09395985644d78c9ff2ad570c0e458e79c0adcc8a382a256dc000a51991908e01ce10e74735c575c18b9" },
                { "fi", "5c3c81e5053c8139deef71ba20336f4acc926577679a51235d11e34230ab782cd4820dc7c67a9168750942f22747ad66d277f4c5e0a417abe7750b59e1666583" },
                { "fr", "f3ca6f456fa0a3cb0cf9a10e38455c6528ca4c66e91d88ff03797cfb7de7a77e00f085c20028684a37a856943a08126e1215e5f044d399f794780b6175946c43" },
                { "fur", "cc1be95d3381ba092431a9765f9be9daa863bca48213fcd0ff8af7fde714aedf216cabed32d0504eee63ee724beca6a61f1c7a2c05479b92e130a0827ae9988c" },
                { "fy-NL", "014178d7842511b287b4347ef71631b36c4985fab90f143ea3967846adc749e18b5850bbd7999c4a2bbb94cb150cc0b20b5a35d2adcebaf99a567ca80979fd2e" },
                { "ga-IE", "4cd68ef89445b9b8815b7fe5c9b03561db245c39696eadec0e02a75e85a365fda1a9a0bffb8cf784f4d19863d0ae053f99772d679fd0b4f872b1fe35e0730abc" },
                { "gd", "9810b50c4eb39306fae6ba8adf61049217b48629c72a7c3b03a4314ba8761b6edff79cdf0da9fef4ed7151599b9a40ec647698d16147e8882e7282fd65212cf7" },
                { "gl", "21fa698ae2a2509f579877b086effc4b1aa8c87ba4482d5606bb6d826f7ae75f21c26e2fdeb4c0f41744422bc405365b5ff276fccb9ae43a79f45cb9b723df8a" },
                { "gn", "b8408bf0fb6765bfcd3967f623fa4dcdce446a35afdafb25c6f5b1ee828816f24c1b5cad6141a6e0e432908722b8bd91bd09e24ca831715daa81125c1689fea3" },
                { "gu-IN", "7ea7d3f6cc5913d28432e620e9dd83899bc10df19e3e93d17e95b751abfa94ae3aa133e8d8a93a0563acf6db2095398e2166bd7b750d8553b0f7d71a4fbf190d" },
                { "he", "b99e2b7cfa57e6fcbc56d1157f4d148b9fcfe30573ceec4d028c8e6dcc4fc1822035b3683e95b717c0cbaf5dc8a2e6b1799a4c837202f3b55b6942c8ac9446b9" },
                { "hi-IN", "014d6f5bab94b569c7982232a9ce49e126b9ddf24bf2007201b721eec32041e28d2858bb7c42eff9edf8da847ca5a2a605c4e29213619e4f20a6c5ef3f6f72bb" },
                { "hr", "a194f874e693e1e066cb1b893876149f9ec2bb671ecc2e5d2efe23b4ca1ea0b21d90fd297a90bb27bafad5f9851fbdf78e07cf6162133ef7a2ef1c3577917ef3" },
                { "hsb", "7e71d5e6d0985fe9e0094615b6dd099333116ee7214197eb8ddc875c05d7375b50d34500ac49ea7fddda9b3d7e0dc6ea896de6b8e4d80a40ff071778fbd1ae31" },
                { "hu", "06d504c1e17155543c5b5afde3b1b4387aea278faeadc18d104017098b31cd607e4cca68d5209ce5daf0f05e1b5214d416f484d0e3ba9cd6babc4bd7d16f48f7" },
                { "hy-AM", "44cab0c2c12c45b9a76681aded1a22f44a707a39db86506b17dd483e56f11cf4994b0372a1a86cdb5bdd7ef6be11a812f209972116d7f9f0885b629661f700c6" },
                { "ia", "3c059fe4ae6fd087f93ce5a4c820894a0fa6f13fe1a7fb2f1f2d7614b3c8d8fb9b6d518f6d8f82510c7e573083abd87d8e71c0e2f15f079d38d87a923376e377" },
                { "id", "11dd8873161f353346458705c8bbc2a8b18021b947c48dfb7e106155e443cb9ba204122a4cadaf6626eff75b0e51e1ca05b8742a6a00ad90e5d8da681ac511fd" },
                { "is", "e419243bdbf8dab4c51b267969d4d3279dcbf1d54e919aea7b2ec4d1bbe8f05529ffe9b652a5da2ad6bbfce7d745fe23ea4d193213b1b60d58ec23dfe37566d1" },
                { "it", "1ed2001bff4528747857d91e920c1b867e4a230b9a5729cf4141b93559c32e4179317104bd69f5d0d5c2e02c56561083ef8f7f449c1c05ed40488e672aa85558" },
                { "ja", "52fa5a5064a48206ba997a5ebd862745b7cb3faca315c54d131750f8d9d8b97553fd539a2e6e7bc76303f5e1656e68fdcc962e3773a863d6db7c1781ac6210f0" },
                { "ka", "fe550506ce955074f47d244c8876b60474de9a36f696340ba5d8b2679e1d02231631f124d25da274fad04c1ca57c697856ea7ed9661095d4bba4d74ffbdc89c1" },
                { "kab", "52c23b7d78c7f8a987bb3324e0dd86dbfae485c167cfaa977a97ed5458cae6b96c88fe8f23077d7e7010f523d0e421a68858ad8f116b2810e634885a9ee73c3f" },
                { "kk", "9fde77da5c1898d4cb63e721c14e7fd46f7b5b8ca76dbca41e75ecb7c24fbe46fed6308d73733a95ca9426b825ad8ffe540d8fabc7ca60035f4764321e2cc3f0" },
                { "km", "727859b76a9a5aaa37b220db275dd45d59f6df72c9a41d29088bbe127eed29d6dd41727b494a7a629dab43838d430ced5b4e7088a55950ee2cfba87629f28b3e" },
                { "kn", "1f4cbecfe19ad0b7a8a481c0e95da106f6b531994303d070f74975fdd60b2610e7326b2636916f11a9f315fe46b6378f89cd424de533c4acb73d371f56c9a835" },
                { "ko", "ad44a73e0c1806e813f02c2334a752297e49cb3513752d940c8b0d06647512db2e390cc79f1de18821d7b58a4586a149175b63021a36f1c23c69648562c01254" },
                { "lij", "a3cdaa619ae4784aec56a8019db8bd56a0a447e0093ef098cc9f182a8b77d61fbf474b84cdbdf247f89989a418c2506092773c6b8d616888cc51f72329c37f6d" },
                { "lt", "e0ac0b697db3ed31229078957ae7d41eef8688d19d0697a3bbd85b8f8cf10f05294c06f2a215264ba7bfe365ccb3da1e05c3c2ff698ee6b72a5696069668fa86" },
                { "lv", "77c0dad2fb7f387480eedd976a5e6cebb35ea7e4d6562e4626b783b9239a13089ea4681de33c07d7e85a1567dd9bffe949c9e3e00447182b0cc9772df95e0f8f" },
                { "mk", "6381c14249d2c693821f7d3a7a95da6e7490fb770e3a27687e9f87c9eac6828467d4b14ca1d761f76c73449f3a94614637b005df0e16fc1d5cfddbb36067eb02" },
                { "mr", "357c2f50a7d0850564f362644679236f4f22c7ce6f9da5372013b586c609e370a07f1b7327c65a226a6306f2bfa3f50f9343a0aec5369e423c2f8a15b683d022" },
                { "ms", "d73a20b161afb13322e433ceb5edea96589090b96546784ccc13cbd82b0e9a907cf2c2bab1fba208188a01998c7a344607a35c2f91c1caefdc77b68fa97567a7" },
                { "my", "d6aade61682094a073a62a0d64bbe6e4cd47ac926958604461eab803404c9ef15df329b70efb067fa74d0b418e93059b1c90a2478697c3b9b37acf0869b559b1" },
                { "nb-NO", "9e784600c61e020b49b03b02abd5366e13478a3b01954ded652d1e2726b6fcc870947f1a5ccb208b4b1bcddf456c28c8790f9f03a1b4c8305638deb165c547e2" },
                { "ne-NP", "fe7c663c9a1237017d87815b35608a91ed8489ed22ed67b5be867d1642c5db32f7068aec40624efcb8252780aa16d1c952d850a2dfda3696ee1ecf9aa826f387" },
                { "nl", "8b99e45b2741bcbf8e657bef99fee3a7392ed9e6a6788f626873b02b01466fa9a9c3187a6fe55794d1f29169a45f357763b9d3244fda74a2c9c2a55da075a1ff" },
                { "nn-NO", "11e52ec0f57d6fa93827fd9be3eddf35074802927bff54102ee3aff421663475032189dc484176525c191014ae0f45555f786740feab1d6eee0e9e6fa1cb997f" },
                { "oc", "e229918a2600fe00eea0586e3b82e6d9be999ec6af7fa6c5b8c95dc1e6baaf6acded094d72cc4f68ca9a22502985dfce88e796f0051cedb9c5aba4e016bcdd1b" },
                { "pa-IN", "7ee213c6c3e35bf0a6971bb8976c3fc1755863b91f61e4044739471ab40ebdc781e1e953dc62732332fb9f94c011275bc4addca38fd71514db0f12b00486e88a" },
                { "pl", "a1b6563e1e52cf1b7178569cf7e0d8bdfef0f4d520be020d5c7e4c46830dd0ed6a9f163db980653ffbdf671bc9e552f301690a12bb1fb5bea96140c2bc4b5c67" },
                { "pt-BR", "06c1b454caf634dbe39a95da9df5b2aa0a02fc5dc78835cda7769603f7202bde4dbf5355b7f85cdd9bfe360b8eb50010ea94d7c0fe91dd8a5525756ba1d6e2b1" },
                { "pt-PT", "2404609b5b63a2d0fe38bb069dd54eff5082556169d7473495e5bc782637288e4f5c527b666e7ad24c7edaed6752c6899baf5e17196c4ca2529cceea251222e5" },
                { "rm", "a16632926e897650587775a6a6236925ea9fb2ff022fd34bb3629fd88a112b3df67ba163f6c35cd726f6398486ade38d56699b9e8965ab65e2a2526c762d26ae" },
                { "ro", "b40a7c4ba8487797f6dae2069bef10b70a092bd04462d7e1f54e7ff1a2c81cf7189a3e4ffc83ed53b1a3db15569f1d02bae0832852fcef70a136fa56ee5c894b" },
                { "ru", "9afe801aa870236325d6ffe59384283d174a1485ea84e7fb2f481d53c906a59aa5550bdffb644366e739987dfe0e5cd6dab3836867855f1962714c2da291de9e" },
                { "sat", "8c8dbcfbd5eb1429bfc43607634d4096da46ffbff995246c93925f194cda297966649c332a03b16277d1b1daa712bbf296a5e0f7eea559fe7088d58c3ea62347" },
                { "sc", "5882959229709dbd1839311eacfcebad9818b261d869f25201e7eee2729797d7c6d1347821fe0c86cfbef42baf6b13be44748af94cd9cb8df94d28fed2c71e20" },
                { "sco", "1b57a64a1064fa4fbfa5cde2fada53bfba071d5a8137884784a3dd98c81d61c47b760d347fc267ec25d9c73ed70b0ea646068264196d18d75508c77ab8714744" },
                { "si", "8272fe9014abad67dbef01ab254424a566fad31ec14ccffe69d57e8ae6dfcf0b464c59cd4a3ee1c10a50860257d65397b921755d54e23609e1147330df70350f" },
                { "sk", "2a531799508771b113540905634e9fe41d8c742f868aafeadf1ece59d512e07cb62612aba29058dbcb4c56dde9726441bc85218368da29b589378c3ad8e7e0de" },
                { "skr", "0531aeeb877502565866aef5723be28b2d24cd9c84ea6db9a7e1c45f101fb4c4bd016169b9ece0f518ec829b6d07c0f13d9427b4cb9021c5a7b35b91aefec568" },
                { "sl", "0a4d9fcfa9ad499194d3793e10373224ae994bd6855d793ef8ba28c6ae76b5676f746e743067d4be9f04a07fc0bf6a521a360255f2c08e7a7019973230ad8285" },
                { "son", "ae461826b234339b8e58750539fe8ea038f961c25437ac3b98c0d44ebf6b5a7566a112b536594ee84a3490509d382dfce9ddd0e4f0ebe7df130d36658aa0f880" },
                { "sq", "ab20dc89008ba5c6da8d03b06de5ea625f926ff4d78f0a8ecb43cd59e99c3307613da38eb9457fd73570752b1cfed0f8f51a138fbac120bc25a4a4996c278dc6" },
                { "sr", "97c54f99bc35220b4ca6e959b8fcf0610d135907daa35108a01dba508047a9b7272330dc76fab535a155de4f7cc9bb0bdd88fa9ba995dc15467a52fbd3e46782" },
                { "sv-SE", "a94164573266ce79816d93781ec7e767f54561fb2bc5d63fcc38830fc54b07cd956f6f7e438acdd32663b485477dee7f3766f102d82cf756f18e978b070735ae" },
                { "szl", "643ac827437a7820a860dcbdcac86e7ec8fe7c0ca9e777be22468581277a8d04f82f3e68f893d01bcebc97e88f86237e820bee0e3dd3ee5d53195bb17b72816c" },
                { "ta", "79fd7d9237c97c78be5acde1d350324a7baf67dae694fc5d4fe1f63370589920842b18ad5d19c324feff102fc004fe7d26a3b3fb1855dc05b6ae4e975ddad288" },
                { "te", "a1bc0aefb43c6d83ffb99f90067d94e37ff2df956a1caedf3bae8f9032c08d937dc9ac6aac0b4e67928a13f377ced630ba5e666c033b4c2828f2893872e4140b" },
                { "tg", "e87de8287534e5cf81597c5e3b27a0fd5e2e5c159cce0e45ccd9822cab95f55a9f377ace633f12cb9f60986f0c76319de5b1f68cdd8ab888e52e360d75f91210" },
                { "th", "a26d71dc07dafe017ce065ceea763a211e7dd50d101d6204e656bd0b8dd7e6cb33f015c9cac41ba8c4b3cdc1d2c1f5a3e1e2798fdb51aafc523168f41917a116" },
                { "tl", "e2b70f9fb3d741d967569e2c69a576a85c869dc756931f4a600b785905b80667df02bdf516b8b212977357a47664b113ee144f57353689c9efb21ebb591ebba8" },
                { "tr", "a5bf61e32e038fe983c91c06bc1ec75adc8df88a1e008b98b066e126913c73abb9ee9feb8815d1068dffd52fe367742644d8dfa6c996f545da334301db32f7a0" },
                { "trs", "1dd2027e312b487a88fa5881803964c5cb2feddfa3cb0f61f0eb5753f830024e995d596c1160960a108722c7338e70b6a91dd229d9da72bf93587bc8bf427713" },
                { "uk", "7f23d7b0e26dbcb6f9ec7497e1722dca663c31b23536c1968db7888300991e5d4834364c940b75446a72554382c3107187b71a70d805f04b884f2e8fe2d01181" },
                { "ur", "88a91d42439e34b6da732cfe84ad893487b879879e869233df348c0e00cbb2a038786b02bf0f787da9c3ed20298b30f6fc5f9cec2784f5bdbabba4ffa86c47d8" },
                { "uz", "62eb849135aeacebd39ee367404524a9bc9940bd7832bb076a048053ad431f39f38dcaef1c4de5742dd4780a7d0634b0c460d589ec67bc8d6cce864d383ccdad" },
                { "vi", "df5ea94307037eb3f716e78426e8c16844baae2001d8d6afb763b24d9f77b815abcac4cf00c4cfeb7bfbdcb88297b2300abe5b15e488d72446e453f13fc7a24b" },
                { "xh", "ea81edef8f6ddad7c6b71998f464aa2aca799c566777e58166faee451ed12ede902c8216b3abf5e0cfd1417e86d2f19891b7d89e2743222aff0948f9aec448f3" },
                { "zh-CN", "6e27d5144e997cbed7a7406302e026a3ddeb1636d346d740694dec7204bf25f6c0dd5b667cc263bc511839c70cf033ae9cc673018b4dff3421f293e40e8546f3" },
                { "zh-TW", "e9953fd9d9ffde71ec07b093d29938396f9588a28980e1890439a25e73fd31bfc2b62dd1cd36dc053ac384c9a40eabc6b0b9415d3028db11a23a55dbebb86ee1" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "60af11dd7814ba10de5af257274bcac48d4f55c8f7eaf0654f63d36b84104ffdd0f8b74ae2ce90404092019e31e85b56bf658b00dfd4d5b93afca58a3eb4f7ec" },
                { "af", "7196ea1fed5be199acc79688bacb2f1681c8c6c9c8e66308a274bd343196e63ba85416fb5d0762ceead60981db9be5156820b8d25971c3f9d337475a9cebe0ab" },
                { "an", "e99a890b5555e148bca80a691a786da10cde3118d9bd7a5269355923d78943b56ed4d8e6683b25f58a8d870edb6080f8fb209c9115acedf4fa33c0056fabb701" },
                { "ar", "25380bd20f0134f72c1b30c577640566245445b70ff29a408c4f78bfe49c6670fb2b61782c388402c7863cf543390771313221aea229bab2f963ccba94e53750" },
                { "ast", "af18d7ab1f4668b7bc48f038d332cd137d159e6f48b87c4a477393c053c111f2b3e30671389b7d48d1b1f14a9e05382f2efab6878f6105d180a3a622504756c6" },
                { "az", "2f6afe3c19376cc0343e53ee4ea73fdf49a173eadc433b685f67bef1a9e1dca6a16fe0c6415edd4445185474bce3684d7b9f7411030f1ab0d252e4ba9ba05f67" },
                { "be", "8bb7c6f47f8eb6ac7d29515bd497fb5316c0a93f7fb104774fb023e5c8d6326db3a46a0fcd11e2f411f325b426a18727adad4df754c547c7dbe9cd34ba212177" },
                { "bg", "1a77c061413631d5594935060dd78f804a6782f499412015d516100004eab686485195e4eb3aadbce3b1ad9fc857bc77a324acc1f71694fa474c38c450d414e2" },
                { "bn", "2039a7a018c7334ec86aa4efddfb04be8763174362f8058c0213d1429ef6ee5210b157292a9ebeca65b3074397244211e97acdc717fd9f80c122759555deb25e" },
                { "br", "0596c322733116d5d6b151dfc4e61531e64da8bc651df8ff63d0b0ea54ca87593849c24ba4b12856d46b90f2c4c0db3d3d481c3153484e2e32877a05b4fce91e" },
                { "bs", "7179c91e6cef41b4359cba09019a4d00acabf01cb7e7ec66c198aa85f4f9329a5eeb202c75210906f9035a7f137316e1b6f4d4a08aefc175eb3859b0a4f97483" },
                { "ca", "cfc37ed1a8c44e0b28eda469caa7c3aea7207e416a4ee4eeb41a893436dce92764e2932aae93fd4919fdd7555bfd62fd13fd1928658baf322c7eae538ca1b02f" },
                { "cak", "80c8da9257b41ba5dc864b1a32493b7f30bd8b497a6ee222b1043758a5b341b875e15cb6e3b62f899e496c79d01d086d7f30f3ab7bb78710d193a9a83d8f9225" },
                { "cs", "db740169f265404a73a537c1f480609d03d4a592fee668b4693f7592791e43ff4a065653daf99f353a475189d63a0aa975caa0360223bb878c74bb5614756e57" },
                { "cy", "6030bec452e56171779397a9f4493145581ac35270012f973332e11db4427bdcc7ab4021e93384254d0767ba7a514966ce3a5e112f214208435be34772ac7d3f" },
                { "da", "53721a826b4e4983570b6338f089d7fa8d611482a0f14e93d16047a30dcd3143fb1466e6851168a02f9090fc53e72eb9ded37150be61325b0689043a0d8b34cb" },
                { "de", "9a15b133891463eb176b6ccca202da146f9d2115b4411ec9080b754235f3a8c80b5ef358c2ff9df03ad06567be5238f841b574a7d66224b89b8b6842988990e1" },
                { "dsb", "c4b5cd53aa5e71a218d03c887d49b87ca54ff966a70974cce8b2b6afc4dee3f20f28d7b433d766bd9a47b36711b4feeab828399fc8eb20f76146d4fd11993e04" },
                { "el", "205fc28f3159ff20ca7271c565a757f416e2f0a9b70567f60f935f1a262ed0c30d64cd10e4063d01c43a2137611129530207c15f3367d6c0f2cf12cc3fc54ec4" },
                { "en-CA", "952fd0014c21a7f4f26ca0264c0ec675cf9aaf75402f3cd0e43de54d5da89a741bd5e59eca3055ed7200c07f619cd30ad497482439a93722a5bf6228f98d49fe" },
                { "en-GB", "7045c5ea2cf00251890efd23f1d64e24be74fdf6b6f336a4493131f10b54e79e50265cc83295e70292e33dd9d677597fe61f1dc996c721e6071eb04e369171b8" },
                { "en-US", "afb4c7e1cb5fe12c84ddbbc11d3d8d02e511c884b304576bebefb642e8fe4ffda2ed6be09ed7f9cccddc8aac7bf22482bac8db9892931ba891d1e2f9d236f818" },
                { "eo", "25607e24cd54997ddd5bf7876ae64e2f06348d9f6f33e0a4af7a27a613f90f51ead92c3ced7d02000ae77bd150018c28ce7c2fd0f7cb5b1646418557b02b3331" },
                { "es-AR", "1a8c5d4eca3dc5b1441ee5037fee120871633f9e265b759d1a6ad4702a0708649f399745c16ffaa00bace2cee0a9b39c819351f81e156768b0419887730430f5" },
                { "es-CL", "43fbfdb225c8024b245f7717b54936a24e145fd9403ee6704640affd54a5edcbc61f4f21468280acfcaba192333cc90d9b0ee8c87b76af60ca464c5b66f0614e" },
                { "es-ES", "321792171afdd2101789a119912ff52343f3e7c6115174a9a4f4ffcfec6b0266261caf7c62b807ef45affc88a085b4e6fadc3548648a5e374505c99e1471c775" },
                { "es-MX", "51d24be7e731b47f7347e83514b6766dd142d30fb8aa3bd931729765b4b7609595c8c41cef9807634843f9bc1c8d7fdbc5c4851e37a5f432ae4214388fb4172c" },
                { "et", "185a59a8be99e2c3a9d1180e8c649697b7edf8e19955b2d6aa7e9135bfcf00b52314dc00e3fab3a83540ece970964954cbb782e45d9289588d5e2e0f4890142e" },
                { "eu", "e9fbfcdfaaa996efe9a15e89c4b3fa8b1c768d62aad6984fbd1540a218a4c1e788bb4024c70ca615a92c644b55337c16ad5c00f5f33dd9727acc1c592e75fef3" },
                { "fa", "784508a9f1497425ce1c5c48516ce76f64f16de21a7bf815c5532242b9597b082dd6247905a3a193a3987e1af6257d1a4c9fb5f87819edb94d719d5111632091" },
                { "ff", "27407f995a86d536f4cd29e5fd419cb354f5ca5b10ca2356d8becc1ea0997b673aa65c07447656d48ba6502d652bb6a0f196f3858e0e7c72ac62b5dd71065dac" },
                { "fi", "be3ffab43d1f5667bda99a62b43521af3665c881354eb00068d655cc87f7f3fbbfb70566ee6fc3a1c205ba9ba00ff8ff1fa422446fb948ac7e7106f20b5948a3" },
                { "fr", "5e5ff0d22f60e18825a36dd821799b180a7a28e070cbb9a1c183bfd42736844d0f26b8af72e60825d8abef9654de52e2d0333eeea4a4883b47c5477b30384ef6" },
                { "fur", "053436e4ce8f3d1a2dcd0c381ffbcbcf8fbde9844631661de7c9f0be16f793420c6c88b8093787f523c2dcbc7819343a1641e7264c0ea4e66c0228eda74cd351" },
                { "fy-NL", "e996f0c559f4e847e0a88a007636156e1961e6f3b5020783356dec3311cacac7bf7246f657d7778b0b729529a0e9c69349032346ef4f2b11cff0df3cc73eb1bf" },
                { "ga-IE", "226d2051da8b211da3fb783a4499697381c30d4d055179cc16a095c0a989b13df4773358ad757d7d6e311a2aad9a5cc7000283d5295187dd4ead06f78405d6db" },
                { "gd", "93db4244af16b895de8d773dfb98d71bb335b62aea3661460e04dccb42bbde79b7143e9b20e99635463871aded103cf2725963e283aeaa0296a74142e389c19d" },
                { "gl", "8529e7fd21e2cbf5c2d9111a9b11c4e877251b19ea3cc5672148bda935a3b44ce40b16cad39a223b1b4f9f69d604386fafd15ae2931773d8908b49f8311ec13e" },
                { "gn", "d833a22d861912cce6569974462c69a27190aa328df1846d90e67bee594b252f60ed5a7b9f8b67b12a0f6b0a44d87896e63a9449e963a8bc276b680458847a02" },
                { "gu-IN", "a8b414e3b8db630ba5dda0f9ea0f23193bb5eb53de1d0b4d60abd6f857dfb84ad91f122c386f411e7587e5177947d432e31a385cf958e6b045190bb9ce1c1c8f" },
                { "he", "af1ec1322f91a97f2e99b130bd331ba31e3f303161c315e32482571a8171476a383b326bf0a4a5e8b4ad2801c3acb621fd6e795a2e8fc6ad9ae8fc0386f7e6b7" },
                { "hi-IN", "d70f452a3b9eb3c0a218a271f77d829e4026115f67855f3dbeb656c25ff8a24bdca9d4e0b20b4e6000e70cabca23cbbf5f73b0c84c703797d554cd51d5136b5d" },
                { "hr", "74b788dc72c0ab0ffc1930ec3da259d210655878551736bb139436f2a8263fe5a54d4a3e98dd62e77567b3e150e75e2a0c64bf91f9ef3e04bfe7cc7cc11ad6be" },
                { "hsb", "35d3389a26d4cd5d0356ab568248546f42a1c8ae4cd2559277a03f598cec89a548565ae64f5666a168cf96d021a97db41d31c8c3223519431000244849c57537" },
                { "hu", "c6e424b53f0894d038b99974367670dacc669b411e1e7b50a7fe65faf66c42ff8046c7b17927003e92599aa5f1c09eeeffcf6246bafa8004dae7b78e6ecc4151" },
                { "hy-AM", "4f96f7fc016e0fc9dba4ec457ce8db812ccd250a2f931ed6b065b6c9a66f26dbdd563957623de551fa1ad1adda0961f144b2cb94bb642056ed0fd62e01fab7fc" },
                { "ia", "49c06360843382a1910bd3ff0ecf7036f965090a6ebf30f864a340020d8c7f83a58c825d8b1a80737a7e033011d44f16d98abca89fdf624b3a290c73e686aea3" },
                { "id", "d6718c2d10ae6eb787034954b37b483246c10ecaae0fd604cb9087c7b35188de5914dd4536e145ec6a4f7c95e8b594f259f03d7e0b9a481b2db108a45e5c0f9f" },
                { "is", "dbc7664a7d15de29a0885efe1e08073aba73033d880e65caa71e98353a00b8a1291f66a9d37aec912de64bc3db9860417b84b1fcaa0ce04a4240d3da0de393e0" },
                { "it", "988f85dba7a1992de7ec9f747ffb2f7a1abe5b54ad322a782b002558b4340076e54984e5346a0e69fbd330b02c9e661cf95dbf19cc1c2b20ca2673d6693b8afa" },
                { "ja", "42a25618af8df620d37f676ed2a41b2342d30756b1fdf89af796896cb48b941ecca267b3c5a8c316b88b99078b8de3d07ce49f427f33bbbdeaef3bdec651a2b5" },
                { "ka", "399f88b48e84c7343ffd28de3f4d8acaf6ce9da1a100b31365177c457fdcd8064a5190ddcc067fb94ab713b62c18891d69a72ab0018aae84b587dcf362820a38" },
                { "kab", "2265032f6340914921664d37b2ab05daab3261e9e6b8b1b740005b373e94a75af6611fb94f5dc08feb46470bca7e68f1e667c6bb0d7b260b3e486acca4294447" },
                { "kk", "c6ecfcbbf20825c87905b0c3fc2ab4a0bebc0e4894e31355c4eef1d8da50b206cbc4291dacd11c7b850ef21a1d9621b72e60048068a45eb4397472cf61f1ffe8" },
                { "km", "b8c5b2a44f8cdb554019864cddd924a9dd0d89c7f4f6fb35234fca2045c92e49b6cf683500384247f7480c726c124af2312ecb60ca4dc4079be0f1b3d50e4580" },
                { "kn", "b084db03eb1a684d520684f8a57b810f2e59a7e5b256441a785d7392e1f1df3100457f9816c75b08f51e96126d43a360ac4326a6ec6efd40b7d3c6193a706d9a" },
                { "ko", "eb36957a001195acb7147d78f02de517df8aa97071023cd822be6e9b52623627ea6c8afc7f104ea54e92c5420492a464b27c9173c003d8ee3cb7491310f5ee2c" },
                { "lij", "e5dd33de508b7bf7de6e4412d4bed1e051140acb7cab976f8d6f7ca3a322c31f3b1836a2da454ef90b39812669af37a242257b2fba829d92b920236f6215f0bb" },
                { "lt", "88497b2d1aa0b110cf8f074eeb9956d8586fc8da087b3c8254e72b829d4842bdf020ae94ba9412bac3627f525eba48da044d73f047de40062d7cb28f6ab9d4c6" },
                { "lv", "c66ee19033b2383428014d54b3581bf5cbfa8d5aeb8f2ef9505d86166e849ae77067adb9f07a50e83adc7c078758a935a9be66ba2dba60e88c08efed74d3e80d" },
                { "mk", "a861cf01af929df54f51ade4b70a3d7c2926ee7d7f12b8c226d6a11c9bd4292ab612579a219539ae0885bcd7c70283feebd6d7617e375351df0de5c6e9a7cf19" },
                { "mr", "d02f1a465aab1009a417ec324eba696619afe3d5bd05a3fe720194cc84dd6f371b3e11280ff390e6c565035a5cf3fa07a135c5f61aa39d3f99b7b56da82e7213" },
                { "ms", "11b84a8d585f27a4df023e6763e5e180bfe0193d49e83607d64c9f2318c48ad1adb004f68932448256a3d3ee6f1d0fae7284ba3709001f4fd912d890242c700c" },
                { "my", "71664cc6bb0e9a52b21de63676db9519fb0427c9dc26c0f09101ce5c4134693502f506d42c3ebbec3f9735dfd6eb5bfa9bd8bd84aacad442c635122f782493e9" },
                { "nb-NO", "eb19b2f0d613657392a11bd00e70e66eb7a32ccc81bcb6c605b3ec9ea5af99f127d780c050b35fbfd2bf982ace32a212f8d9a1f63099f4bbd4ff1b265005417c" },
                { "ne-NP", "8ca7d8bad5bf284e4bd4f86ef90982bfd1135f1bd01408c47f20caed14497934537c9d618e209787aac520efca281b2091a0fcfca63d48deb95c5cb919fe9638" },
                { "nl", "7c7e06cd18dc833acab4794ab6fe76dd89918a76f0ca1a9be7cf34238a91a7016b7cec9a821e35de92740360dad046d6d482c8b630698088f05e25eb1794f537" },
                { "nn-NO", "5933b669cc283bb06eb4afcdaa3102dfdcb03946594df5e433a05b330937ec0f036f21aeb1e7e5e990a1e62ca7291259bf8ca0458cb658911e8b7eb31282f109" },
                { "oc", "1cdb6b4f50f87e63c36ab1f63e269cc1e5bdf90b354bbf98ab854ed69d5a44db167af9b71021e4e6f51c16edad9a39b9969ea187e1a0ae6ec41734204f8dc2aa" },
                { "pa-IN", "36121610f979c232ada8840deb1c4020f21b7b4157d074cc4fa15a7fbe01e3581ef22a8614f3531dad885aa6964aca21054c4d2c7373f09b92e6a008e3c2b9ed" },
                { "pl", "eba96ca1afee4ef99e2734b8a3bcc02a42bd7f76272659c17fff2ed2e01c3d5ba99b381faa55c0f972626fe8605ee8953c790575b14bcfbb31650231a815e7be" },
                { "pt-BR", "71bae68f1f53a0449b952a3c6fa60c213111dcd316654a5f763e51bcfe06dd2f969ab99cc6aaaf5b758f80e435460df36fc8df208b0ce04e405fb0f1696b6cfd" },
                { "pt-PT", "3deedc986516a7f95d0681bc360d20ecb5d87c4a3794d66e5cf0a289eac79fc1808e2c063451700adfe38bc823c50abb4c47817bc782c65cdfd083c99e5608a2" },
                { "rm", "38dbf5e1fc77834882b2a9983ab868c91a4f67a84079a7425a72412df6c700368b2e05ac59d870f8d14fed25f7e8b79c070e4c6bd788f15e2841a809923e8bf6" },
                { "ro", "2ca0f61476b18d67bc5423cac68e0690e53418c652f4b89ab151b827befd843d787cd7a904ac3a7745ee3c60fd5a696f1f40a2a43d27f29956092bd77d7b8d7f" },
                { "ru", "46e38fff608751ff05fb239ba09e262451f4a7bebce80c63ac72b79f127150b01249e8ba5cc2242b5e0893d9ef25974a62b7cbf9fbc20651c2336ba3ad0c4459" },
                { "sat", "d5cdff48e374f1a1ada0682dc9e4a281140cc6b9528111819135882ce46402a38f751eb978e7551da15a78421df112ae6d542e5ff254b064cd82aadb70afc29c" },
                { "sc", "3f01235c72a3c7e00a4da5326bff331d1a73149530fb6d19b3d9037bb4112901afdd8eb8800de81641141c565081805497637c4997a231b98e4b1433e1f3c767" },
                { "sco", "de25a2a55665baea6b2147ca1731390b30da7de495dab777ef943ccbe9617e0f99466d71a058574b1a29ca30d1935745ab7cb337547239d5e1d85b790a2c26de" },
                { "si", "70ba1c0821ba5ad2de66d8e4b9642b9407dea0247d946e131783d451f8a17ea9fac4b350119f2c2365ca1c17c78e5a92efbf7ae98051a206516eabd60bf57e7c" },
                { "sk", "6b14abdc6b5b0eb4d791269032b26cd936fdb1fb203d67770f564e0f5f5ecc99269a77573084d8d9e19ea7a9b0102bae820597841ca59e56f538dcbbc2d84c94" },
                { "skr", "5dd4ad9100df5c79df32e415549baf6229f0393a9a782cec2a31b2fc61f17557b4ec5944efad2b615798ba6c7660d18c43ff17a462f6722bd3104150af845866" },
                { "sl", "30429f7dd3c1143110fb288d1cb00f2cce9c520dc4e40945e868e85089d3a3cef46971f3ae10fadf31dece2b2ed287f639257bc67a9190ea73c090cba9f8e91a" },
                { "son", "249ec97f9f02fcb374f423b234e4ea1230b3c0689aa945125a45b6eeaa0c4bb029da138808aac51f1c2c8f1692066252cdcc39fb9f45c7140774cae055276f0a" },
                { "sq", "cb39a6ff3b83ec7e2862a671bcc772c2eb93a4e87f4bc9afe9a91196c24c48d882fa3c4737700de5e5574997262686b4c8e093b811cf3a1e65d87825d2296ca0" },
                { "sr", "e9ccc45fa226cc0e9139caf82df6d0c0b838f3f1383845593071d43923c1fb5ffd6942e1ab05f1a8504b74a3fcd63f58e98d7b3124a04ae40092a0280f30e8d2" },
                { "sv-SE", "9fda69cfc021cc9630dc082578c33c83bc4b2535737855662da8d623471e69292e6e5cd68c5e752199a4f70f0d3de219933ba4ea2387aa66cf8ae3d47cf0e81f" },
                { "szl", "f0b0de4bf0412c5a9a64d75ac1e832ce7a43b7d481ca3109b0d0b68f909a123cb8f625a7c70b27bdc7d75cc8f62d1365a37273214e0b9aa722259571565b5198" },
                { "ta", "598e5de8d96807c6457306d5611ec9c9d9a94bb7c05aef0b5cb8f39776ef5684552e6f1cf777a8cd9aefd0dff9c5601e5abd688e3ad19f76c688d2ee38630655" },
                { "te", "ec7c9aa5370c86699481620f719d94ee459fad72290d3fa0ccebeaab7e3a5874542b795baf62ac6c6ae5174d57ed2bbd9925081c133bced3801b97ef3ebd7204" },
                { "tg", "520ac027d2f721e6e9b431421f93c74eb5b41d34b93a1ec353c199010cd400656ae3cb0eb99668f0c9dc44490603ac9ca3738e8f96afa7637a9a642e7c14caba" },
                { "th", "c5c2168f550a3c9628be43cc624831e18b2dfaf5b9ced7ffceeb9285f6ba5cf192cc09fb6deea3d8765f5e6f6361df7312e17f8f4fa1a16eec68d8eaf1780318" },
                { "tl", "8b3a839c60296589dcd13e38cc11f6dcc3f5bc2f46bb09668485cbc660305df73bda515abd3855f70b22c0b871b6081cf6d9ef4c68b836bbd52d88e0a3c56c89" },
                { "tr", "9ba01506713b5dd63a652e46dbce2246b0363a161d59f6fc43191626ec3a44735d7cd2bfab6665f40836ac489abd43a550ebacfcccb765a364f81752870fe60b" },
                { "trs", "86b67fb85274a89619a2a8275cf65f254f1cc8f739633a4331e278ff012e8f2e2fedc32dc7be6f24685e27a1332df1c119142cba7e70c5a934ba7fe9ec1ec781" },
                { "uk", "73ce8f95fed90c6672bd6151af7f63c30b58587544195b40a32723ad359e20ef713f2dd09f75df9606701c17720efd6c71562780802359f4edcae026307f910f" },
                { "ur", "85bc2c3fbb7a185ffda19e362b164ac2547abac0f301c114e45544bfca04a3472bf4cf4f2ca0adf2f95d196f0d950de96af7e6c7ba580c032527e9cbb522f1f3" },
                { "uz", "b372a1d5e3baedd71d12d6610bd6d3a0168c433eb2344a34c3fadc20d3f374562dd7d364429023a2a2b58a9b098be93f46364c9b1c7f752422930d8c255c0af6" },
                { "vi", "f66d5f28899031f427424f9fde60c73b65b3466c84588e25427c17f20b14c7638a1d81a5b9c9667cb00359294c00331f141cf0308006b133066c560b256b4f22" },
                { "xh", "b666050a8381877977c883be878d3182d43ca3d86cee5da5a979a78aa056e76cec7067fc058719f30301feca363f4e79e824bbc1af530987a13c4a202f2162cd" },
                { "zh-CN", "e4826f9ef2133899351119222ed88d6ee0f89a7a336ca0cbb7fd806656631be956b92ff420ede99e833e07156cb43c652336361c96083933e3e8f4ac20f5174d" },
                { "zh-TW", "6927d4f18ae1796e4c9d50fb0c51557a54e3f9fc33dcd050c8308ae03909c489973f5fd179cfe78da62ceaa30ae163d6ee89497fdac9174e1181b3c9a08e6fbd" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
