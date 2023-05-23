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
        private const string currentVersion = "114.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "ebd3b0adc3f2d922e35da62198d4c355ae98590e1afb56b7b312f5e9014e840dcc9404149fccce89b2cd678f01bb9a66311f56e48cdf2ae067a5cdc2c649fe73" },
                { "af", "57fd8665d0b14e8f5f819b29ca7a24b86c65d6889615ef89c39ea8c453b8b48ddb57daee0a8acd92145c64e441a92875a5c65a8382084edfc1ecc869ec8068ee" },
                { "an", "272541aa489d2fb9f52f49b1e94624ea11c0f722ec203cae203c7c60ef1cededc1be1848de23d2e5f5e11bcd6e8d9a061a31446a34089670dc58c6bd3fda71a9" },
                { "ar", "44ed0fcbcf5e5e1716290a2c4c09caa15c00b024de20805b57e975169a6373d4b04066001496355b88bc6abb4944f85e30f50e3f2fcc2c6093e63f8d24c7d68d" },
                { "ast", "870739d0c36e9969f8e574e7529372f8b0733bad8a85f609c96b6084af508c16cc806bd76c37abbd09470b0cff8fc729ab053abffed483e146c08315e54f389c" },
                { "az", "1bd1db6a12555dd74bee0aad562cf436cc5501cb894465306cd5d5736beb25e5de2dda93271a06d4698969e4c3e8b1ab9aee16055a091c7385a65647110b8ead" },
                { "be", "fc53be6bdf49a5284e65e0de9ac9a9607fb745371a6f659ffc593bc3de431b111a3f9c46f4561e1eb82e60fd8489cd12ce1d733388cc0fe7dd3b056066c8fa99" },
                { "bg", "b9b5def7b51e993feb9e0895d31c2157049a487f18d3f00940a673912675e5056942131d8d04e01ea3545cc141d3134408da9e5a11df1bf4c1ffee9b2da3e65b" },
                { "bn", "b3a5c0f42b1da3c1573b800b65bd3aa9dd3ddb8e464deb61eb28c7a92024da49aaa2c330f45079df3256cdca4bbfc2501253b84fcfeb5b77ff3ad375ee76cc22" },
                { "br", "6d7b9d1d63ad89dc8eb2ab9c54cda33f0c5e0d3bfe5feb88936cb74bbbb51d4919598a740836f8954d4c46c6016d59edebacd8cfda0f4462840182ff05c6549b" },
                { "bs", "9841d9527faff3d8a359c1eb39c38b9c12d458a3f98a9b084228763bf51c11392411fabac939a8e817b0690b376b27fc379fd1a8b438bba8a48e1764d3094e02" },
                { "ca", "16078a2f0001d7cd1f3e7fd862847436ce034c46ae5ec01381b643aa504aa73ab3b92ea1a200f9652fcb8fc48ff6af933250e0e2cbdb51b76a73ee9d1850ec3a" },
                { "cak", "9609a551e758083b510c58d313c875ff8beec376770d2c1f45494c3dd34c18d73ae4ed39c1d0c24c199e3540153a01bd52ab9e8a3dfae98d48352cf3baf9c5d0" },
                { "cs", "f9b5f1de4b356216880ad602dea301b593f84b83bfaf42256a1e1f3c65cd7e405897ebca686eb323ba2673fcdc3acefb449b5d4f0c48b03f553d7625cccfa7fc" },
                { "cy", "f504fd40f11a542f2352f1edee87834af628a57d16080ea38bfab3eb8469f5067aadc4d9c12270ddae4f038de4972a7c058057ef6ad2ad0b39260ab057751e22" },
                { "da", "f16238257b670b114c8fb618797bd0e6920a719442d6354542142112196fbdf25b60fcd2838dfed71d734d2661cbd6170def5b65af56293f37d52898d0d80099" },
                { "de", "4ce99afb322baafd718da774b920421071a2780ad6bc098a02af5e0b9cacb41cd6d47af1dacb05b1d410341e15d7fb9c1068c2cd0d8511445549c9a6c0b1aff6" },
                { "dsb", "7b183e24ca72149fc13c1c38bb70bb44d72493c20e4dee47bfa1c5b9e9edcac413d290a5605a4cd5af87091cb7f6b8184e17173b4f54c26573510c6227ad53bf" },
                { "el", "3f9fec69e0d548a0ff9c7eb99b7949c1bdc48d39f2fc0831303d70aaa05c290c1433a4a700f23aa08451f1e32c27243f98e62fd9253f6170370b93c13395b8fa" },
                { "en-CA", "436ddfc87afc1f7764b8fc0f207cbc638358297d1504fd3173aeb20968d436becaa67156ffc1eb9a141c38464a4c4f929515f054f91bd4371b896c829291e070" },
                { "en-GB", "8ea76e668efcf7174d643127ec199a037a740d09ff9ddaad231355b5fbebf8bfcc171b6a46406f60f4af038cd4e9b10a04121a55775b8126152f1e3c1b255178" },
                { "en-US", "42464c8868e37d7222b9019f0fec295ce24b15abf80f695918488b3cd7afffac3f08fa16cf2e47da293451faaaab90a3b40b6336fbc8be3b69ce42ad5ad9fd2e" },
                { "eo", "1b9781a4c675fbc19c60fe409186626cc7ba292986320504c629e3c93a27e9048564321d582123bd9936f3580527664451cc2f2cf77e92c34008f538f2764db4" },
                { "es-AR", "0eb1084c62cc5a7b0574ce5aa87524c07477c3bae656f88e8755385f9e0d4b1a78a2e914bf8492703e0e107c7963d6c28b17d0a08770922b970ec21e9c79c9e6" },
                { "es-CL", "c0ca447b21aadaed05618a9cabcdb28e08d0c485d02db26ec96398f4409a61fc3052b99459a1f550faed7032969628ad832e6e92d8cefd2075505665f0f7496f" },
                { "es-ES", "47233a8a8c051c5b5731ddee30890cdc3943a9c605bd1f15c6fd84acb37ed337e481c090c74ed331b608a60bea2a16f6f14a2fcceb3a4477700d602369f9e699" },
                { "es-MX", "a52253e66cb071a9f1d66eb0a6705eb70323d094ce10663a82d48fe86e05efd57caa4141807d62946cc1ee7ed499ce270071501ddf9cb7e23d5b6e347647f5bd" },
                { "et", "033c90270e4a5688cd4442a90ee518ebbd620acdc7bd192f0b7c7a5fd5f579ef9c288183b804011a71034bd37474bb9eaff1b2c0932c083f9094b4a1448b2dbd" },
                { "eu", "33aecd55f4ad83eb2878a8a4ebaeda0ab2cf274c9828546533094adecfb65a2a1c1dee18cbd0a21033a4be9bb3464847fc632a48316f4a67b093ab4d18b1750a" },
                { "fa", "00d51bfe4b86b10bc2d44b4968739ca28933bd41e16090440d92bc9dc6efd64c313d035bbe2d3ffb7e3537375c883a7306baae3c3d409e25ce83cd2de14399c5" },
                { "ff", "0f48490a83ad85a667af4d0c19d14912e64cf2fcbf633f7052890d99db96c0813f1df2d2c347fbc0a841ae9d3e40a17ae9e87b6f790731e5ae00e02661a83a63" },
                { "fi", "90dfdd35a4c9601e19a4a376644b5fbd722d7d7029095bd30be5a36bb79f2430cc0dbc2d9168bdddfd06d67436966302b2a715059f45fac211dc69213eacb27e" },
                { "fr", "bc7f1e9c98b87f1bafc29cd55e874cbfe387310b723a272173f69343ac8e30bc37f64a59d11c6f5abf1c497bac77601d744fb8a98130afbb8eeda2e8fcb69a92" },
                { "fur", "3b9ad31b01fc1ec339ad9973771584a18d2687a09cbbdbb68199a5ea7aecd1651afdf969b6a1aac6dd7e2a6e710887ab8b32185cf3d93cd2bc6a8830211f8ef9" },
                { "fy-NL", "d8c19d06ca4070650b5fec401c30668f1ac109a39df9f03b1edee55852d29d1ff24c5bef03676c082ca962e84daa1dc1b22a7d0d0644c04ed1956440f1a297e6" },
                { "ga-IE", "1c78c50ceef5f6e1fdbc4518a1f7dfdf6e7041a4f21055b86992b46a7b3f2a17d46070e35b34df39cfc7bcb0152eedb394f03e5ac87593edaa5f1639cbf15a63" },
                { "gd", "ebf4a06be7824cba77ae19fd866fd499a2f37aab3572d8385ce85d70fdaa7b40b9c3127d0a16deddf9a65824e80cf9c8a1cd12fb21d6244a2400594976322932" },
                { "gl", "48c1b9461848bed057a8e8faec205ce59062edc3447ba235457c7967bf0797a1a9d3bc8160207357345ee7c05908dd78b649e2adf7c5a69db6f9a3da100d4f17" },
                { "gn", "ce28fde01fb8467221e889418bdbdfeb6660d5285a193722a2115c84645ee59bd409290701bce10134494cf977b753e50d113b45a4c96fd9e4b8c0c1f14c50f3" },
                { "gu-IN", "da6d202be56e05709821dc91dbc7c17ffa8e8fec295f4603a5d14a639ea433f44557a7c811af0221323485b3df0dbd6f1490d9d4e97b6bb0ace170521e21556b" },
                { "he", "51a78968541b0c291d2549aa256d7f87c0e7a8584e5f53c9a35466e84ddaffb18938c6948da4be03b8a221552e6260a5c55b6924954bcd72374354397285c2cf" },
                { "hi-IN", "eab794e7b85293024eaf061b61fd9caca9a256a2d1e886548c34571bf05db3e2dfa15f6c55068d8eaebd82566f012bb3fc3df234267e190dc3157d9eb7aa3787" },
                { "hr", "4ce1dbc94c58b256573973cc59fd1c306a480565bfd9d762ff609c6352e30841491770177e3afd7785fd4038b0fa142a93748a84039bed6bdbb9f5014fd18875" },
                { "hsb", "b5e3e6526bd8106a10e72e8851fcb01f839ee87289b8510e730c041282feb12c0d9bea5b7a3b259d725ddce2ae7c43a9ea6de23090d57f6436a6150f490f7222" },
                { "hu", "19d2ba843e08196b18893c2854c13e52d27be3838830b2f16dbeb67228a16bee68153fba57c6ea252171590eef3f20af00bfbb9b60ee1918a3f79fc4bf203cc3" },
                { "hy-AM", "3545da35436ad92449d8f5df3ebddbb049f1343a505c3dbc351a0776a07e5f2b12d58521a72f6d3096437275b48ab725ec59eb9468e741202b9b43a92fd7c86f" },
                { "ia", "1eb0a73c4601f023556eb601d9cd0dc5cade0430f7e314ce15e8a479fe5928dda2c26392facfdb248dbb07221e4fe8cb911aee956d7e53a28945d0ff15dffbc2" },
                { "id", "cd7cbdee4f5a191938c52e2047e685240f1365f1e8577511627989dd627cf60e76ab0e9908af0030294492e32089128f3c1c921ebda201889928be6d5e13c315" },
                { "is", "4822326cf5930ab4e99991c4c0440eeca2b45eedec26297a08be30753ccc45c68cec21f7f8144b16b4aa2c1e3af53cd6c73105ef19bd6ee1e3d086881fb915e0" },
                { "it", "7c731320e14ccccdd70c2cc5b2d9632fa3c20bba649d3ad208b6df84922360427c02788cf09e08b0da6bd4c42eb1c017bf8f30925c1430074ea131c084c3c411" },
                { "ja", "6da0fc71138fc8063049bb2580fee9ee2c116d8bfc973e92779a16d863a781be6f1798ff7fa76ed71e6706f732ae90492554ca43e309586c735c2f6d868d6261" },
                { "ka", "51760e3cad1f09a656aa2c6a662288d8d62be2fbb3547a1efd02e8cb25f68cb6d660d34b7db05e95e92edaabd9d70a36df824f09fd9028d857398c6f8e181f6f" },
                { "kab", "f99e7a78f1bcf2e990f9cc3d96afbf4259ec326fed63a9ab0a40ed7af4e116ef1f19b3c85a4baeeee75ef9747aaffba289a89049685adea89d71b7ce9eede6c0" },
                { "kk", "3069d905c54500336eabede941bf098fe45fac57ebb168e0aad0f5491b50710f553eb1637877e3db45398306cf5aac4bb7634e40f612833485ae4ade77432539" },
                { "km", "5554b83aaa72d7d59c28582bd89b789ee13609d029e5b81f39301364d5b213c34ae3f171347a7315d665f29604987669fc0a7987be2a46e8bf59f9379ea04c5e" },
                { "kn", "7bb673473a8b20085cc0c028cdb24dc558d972e1aadb36026aff3c2e0130e35e205ab48339ad8dc5f289601ca6e5353d2b37de3c46d08133948e5616d76d0bcd" },
                { "ko", "93bd9f014e59463ee92bdff65446107067fca123cd6ba2e2cf813f077bd33abdd9b4d628410b2317d685b7d90197f9b6e204738f62bc343cd51002a38de2bbfa" },
                { "lij", "3b9007dea81e1afeb33e11277a298d9d99208b5002efb339f479b2a78aa9abbc8918b68f7dc93aa6737a9d20cd7b8fe60dd0223fde31269c70716ef92aeccaa4" },
                { "lt", "89016f5a55433e99986e4828188618e21eada7fbb659343290345125133e77a2c4885a0085501a7b94cfe80ce649a3551aad979aebbc3aec3b3dae4efa49bb01" },
                { "lv", "311c0da6263e1502d76eae260cb59d76d70f64cf03662eb598cc91609c57ba68f71d3b7b5c1f7457dd64591bd9baa2049adfabcfd9aa444923f8654c599c1cdc" },
                { "mk", "7478ab8819d430c4050072eed381606f78b740b110003519951747429f37d9343891a4015f28250e008adafd5ca59069735b287954fbc187dfadf8074a7c1eb8" },
                { "mr", "fea0996e37a126240376fd5353969cbd2c8a01a323a812b0e1e3c955667ebd256c88a229b48f0a931c128c545ac94ba6bbace133e94d70d18577120b6d439297" },
                { "ms", "2051475b05b9ff90999ca4f9863165921c675896252d1b23c3e1dc1b70217050a36d544cf3baf06ebf5b4cb063206d4acfbdaad05120973fe94f8daece30d919" },
                { "my", "c3ffa428caea469b0ba47a8e8bc7f46af0c9256b0a07c1ff6137ff8bfd664f26c2304b3b20d0fefeba019539d2686df17ed2e8042073412fbaa696ee92d88905" },
                { "nb-NO", "cb2da4ec68f3e6c6a81ccfba20cedeceba13c57b8617dab5d5cc0d698491a4be2f4356cd9216f28a899a92f8530d9794fa8ef3833f2f3abc688fe3540f169d7a" },
                { "ne-NP", "5352cf60726d6d7c45a27ae1c1acd95ca76376cd19c765ff4487cc5bb11968c6d4b9cd8caca279b452bd68ae91b6d15f05b4c2fb6bc782b989be5738472bb5d8" },
                { "nl", "9cf7859c20c3a699cc6189b015d62f952b616ab75ab7485e6871fa8e4abd3509c25b56f5798c51e07ef7eb14eab20e3c1f6bc5c09e65e064fd932960308e0e90" },
                { "nn-NO", "4a94595e08cae4f98bb5df9939b7f71212b4b385afef78352dbb8be6053e4623047f160e4a27aca9ade74965bb61ce6583c6c61fb8f8e396eca524e0e61115d6" },
                { "oc", "107234be2fe544b4fa6d1dadac9e997c39a20e4500d7639bac0799d1cb6a447169c3c0c48146f8b149463b2e6b73d5de4e0804a90dc1658882610a7da5293c26" },
                { "pa-IN", "4e1565dfab12e368b3551aab25df800e0551082c2e8550bb8d3b7ce02a60423d2566331b43f197771039964d012950bd3a413e23de50699dc90f9de4e3510738" },
                { "pl", "ad4ced86fc19184d93e7fb4bb0261329ac3241a8243b15edd4b1718b813de4be24923cac49e27c251ca4acbca923a2ee096d1bce99d3893f0e7fe4abc7b63e1c" },
                { "pt-BR", "64a37a166ef1dc61112fbeebdbdfa7c03b073a13b455b322c6f9a6edd36b9232e5c328204ed8dc55cc1a109e029be44d272a0a48cd56201b4984c9780e0a8b84" },
                { "pt-PT", "86a76c724a937c80f7b869e36dcf1a107bd063162e5a6e8d8ded877ff217fe17585442f22ee23b07cf0ab20d374e77eebe3f290307027855052243eb2269c200" },
                { "rm", "463188a4a17b333ff75b649574e847140a507359fdc00a7b9d20ab14d5032694b894929e14191fa1e19b0adda0812847a369c7f82ece99818f25f12eed5ceab3" },
                { "ro", "3815cfe8fc95e19fd6c012e1bce61ea19e512ad1804ba86cf47339de061b2135d72dd90ca7feafee60760bce1f6b8ad2cf458ed1c0aef816f9d210029041996a" },
                { "ru", "271096f2ac20a9dafdd88540811bfc8ce8fdaec32d5ba608ea90f3bd32750abccc2a4fe0bd233c4e9080d1df8d8495eb05468c69f944e2c0ec470abfca7dfcb5" },
                { "sc", "faf460fcfc19216e0d0b2806a909772628bafe09d3fed7010841bb124f763c85995ae88f43528b7c0c90a22c9ddf9735ce4f2795d095b61c5315668e76f848e2" },
                { "sco", "8c2735d9562ef34eae2665ca9a76c242c89c7143dbe803a44707c1a3614668ab82b3309a6b367482d49d257f2968f5bb983f7f3a67cd8b1d1991516fa0af2d85" },
                { "si", "1f34b0989afd393810500ca0c85944b8922e6517dfaa04783eecea04cdfe78170b06e17789392ba2b9ff7f61db858801e63bbceea056f72cc5c2ea443114582b" },
                { "sk", "a27146e27222a849fb8184fe5b3a1b2d29dbaeea03746ac86a98b407c2139f7802889f99841d0baf7246042a71d902404a698766c7bb9e3635ff21d2f8511630" },
                { "sl", "d5569dd4998c2890f5357b8ac0ce23420f2ce4672779050ec49e4d70ad5cf524b3b85fc7f787df6c0172f73e03c880e544b39b222b74f1ac4085c7a3bc446466" },
                { "son", "6ef7e57a0f83bc4abedc0136bab3d2a6c6f2b6d5c281a3c602c95db9a4cc2e58c867e6de8f366b7fdb977cce4635376393a02289899c0992d32000dc9a479173" },
                { "sq", "e2076b575b400d0f0d84fd29caef5610192da052a14c8a9aa2cb6f507f561174bb0982d06a1b87d684e830bf462d4072419a23d29a57d39e948c892a0b886f39" },
                { "sr", "ee16b55ab3f556930e285e86ab600dd5f196e5c3cb0d32933c1d65db9d19fc4bdf6cdd8469a96250d1232feb17bfdc9bae3a212cda08de90aeb83b19e9fa685c" },
                { "sv-SE", "7cf6aead1584bcbc55695f15760d2fb661e0f4dfda7660c44795960cdbf4763f92f9a72b3e16829a19653d395b26d2b49d447a950e6f48914d176d7d814bd6a0" },
                { "szl", "6d32a2afee89639d2063ef002e90f608bef54ead23679937f24f4fcac45d40782b73d8f8b9fbd0f3c2d642c9eee1463b0a09a91c1386dfc16e9ca128c396c669" },
                { "ta", "d21d4ada889ac5c9d8522a8a9fd89c6b446f255dd9258068af5088450b8364592484909349caa99af6a747642ec07f8b92ba32fdd98d3d5ca93a8dc59ebbaed7" },
                { "te", "16abe27886d652d556a4d7fd9f35edee5c9e305b475f45ad41a15ca755f58de17ca45c8cb32a96623db68d6dfa370d2385e8d912ac38afd46487ac33997f8b20" },
                { "tg", "392224222c376a8a02165fff581efb5db14bf7e326597faa5b4bd4ff0ccdb276a77d4bc5459579d8af6def9b16925d2e73adfaa3e5b4b53575abe9a3eb4b5670" },
                { "th", "04dd0ffb17d18750a1f721268589aa808bbcf2799f6e5ef9eb38952011ea56ca46c05742dcae435361522737e76f08c34312cdc816d77714ddaec3f394acdf3f" },
                { "tl", "cb298d17985f776b8f34fc6efda7014f5c885d0d763a7effd4a9a101b124b3442ae67bbdddc83a67cc2ac9f666a4b9d13f3ae0802c9eb79df82a74ec3124552c" },
                { "tr", "a21fb474ae1b88b9b5e55e075ad2673f3d2d78e0fbe7634efd89d71d22b6ca589ed4585545b61680a076b4c2d7a42efaa0d1c68ec7e6d6ef87fa2f05745b7b2c" },
                { "trs", "0d2ef8e6274bc63341880a8a8424196ddd5d72629820b0f7dcf2a46d4d3b7c2da33ec9e2a2aaf2dc28716b07a5829d4cb92bd1494f3969d7d0c727150b93566e" },
                { "uk", "c09995669efa5c28adc8249dd9a3146f3ffdb7124549c82a6b07c9afc4bd2d9a52a5a2e78921d19059bc0485bc67cc81c0e9b79dbef773a687bfc1a760ae8c77" },
                { "ur", "3fb32f4868b127069501cda80abe2b56ea5049c356bc7700402c9958ad787f6241adecbe9b496d30e219acc220d6d762f30596c8907e2880026916a2861e3e1b" },
                { "uz", "f82ba7fbcab6cfaecc76aae6e65698507a516ac652e094f05579f5c251c21e48166a727bd5b3a406dcd010d1c742254d602d3b16dc80bbff5f0e1fb1389f559f" },
                { "vi", "e22874f6750208f53c0f39a34e91ea4991e88f8e2ca9013033497bbb3dd898a995cdcd90a9f01e2c0f8d867d4daf489af0883b1a07a98e5aaa56bc0b4875a4cb" },
                { "xh", "e5fdeebf860c7fbd489044d868accc95cd75f7f88876968857768b1439a2a04a587fdd001b7348e658fafe777fcd46b405502af1967d6469b32524d6b863ba04" },
                { "zh-CN", "ea66a498696d03d063991312fda181fa57a8fa29a40f4df3ff35770198223ea45cd294f4fda4bff451215d6c1c6da361c4344ca5dffe5c55e70fe4fa4d9fa53b" },
                { "zh-TW", "dbff10a2c31e33bda8fcaacaa5e40ab6e24b8ef68bf1fe760d152cd177e6bf88a160418b5e144dd254e665f6692df40a2b47436966be3369b620a3489d6359f0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9845582439b11e055d332a4876b7fec08e91747a023de531b487a3ad434799b6732f1a971509d97c7949b0d53c08585172469a7825e43ba081706f266c9b818b" },
                { "af", "ac9845a29aa1113d086c3cf2d64e2677b7e6025ecbefb426a2f9bf6f6d18bd6d6d087496fa8b2e997384d2eac37b5c1a511be36e262cb3b1a00425dbeeced706" },
                { "an", "c433bc60fd1507ba60d5b58b4da5826d5859089e676a939051321ae276d43a126134797423a4e4c6fca3958f228cb9ae9169bdcc3b878232330987d3c92e170c" },
                { "ar", "f624778c5baebb4da4b28f516694c25312f10b78728a5959016644335e20ee8ce6598bb406cfafe0c9cb299513ed74d120509c105b30a3ee748d05b964ad2b10" },
                { "ast", "a4e5d4e4f2415581a7ddaa2d6e5a9de7af22c846f72d48eef4b1621173f07d85b67acf0ed0dffb72232bef3232c8b8ff556de30930c5daa2d1b92211aefb6e80" },
                { "az", "0278bcec7a1632627307eca2bd6c88695cc62cfa4cf5ca52f7222b08b97871532d7786f972c08bc1fbb8cf69a3c175bf7585e761ef5665189e1777a23897061f" },
                { "be", "877211e45ce59a0ec27bca9496e0062720073d70160708d5ab81d7c46ccddd6c6b784bf94c25d81a098711cdb10ed21a411261a4a10bad7ed00e8270ac1edaeb" },
                { "bg", "9713a127b4d4adc8d3beae94dda517f8da54f41d99ee51263eb753f467c802c7be56a261a07c52c413694d0daa6aaa6a399d0f46d82d1974da9cbc420bfc9154" },
                { "bn", "ccaddf6176841cc5950a1f02897735919b08a1e7ee8ec29a503e55ed183f8203e23f832ead29355887efffef25ec7b5b03eb017cc73a898b63c278af7dbd9556" },
                { "br", "d2209e85aa89b8c63775484778d45f7ddfed8df8948b8ff6672f4d898844a0f5cd3b2cf6dc43fc6b9c3fb7f5d3d1a9ddbced97f215e771457756e76046c0c1ef" },
                { "bs", "a5efb3263e17ccebbdbc713c451c760f34d40ad17de1754b9f0101775b7029b0b684b7f20ea656be02a89596e5eb72f820cffce725d715d696309c664b9ccba7" },
                { "ca", "606cecb27ef3f48e79903d7ae362e2f413ad2882388fbbcff2ea8793fd12b7cf4f6811d9cffe25410c31e99ef56dc901d820acc7f9331940ee7a39ff8bb40d16" },
                { "cak", "6c76cdbbfd3e61f7082607a00d17c979bacc0ee35442b5a7b63877187d0a4adaac2baaf12262a480c81f84e00fa3f29d55a730644d87262900b8789348e30655" },
                { "cs", "0d6b1cae7839a6569fb26a3892c2a36829067ed0a2ee21a50121cbc4b902a1f044d4cafd539402fa3c7aa515a9554b774bdb8122b2cf62165804ffd30f703f3f" },
                { "cy", "79afbe0af748e5b7c10677f80292dfff23a3c59b454564d24667714595a8e05734b57a0a2bfed82c0d5c500f67f07dda64364a9c2563b1f286b5be9d263d030c" },
                { "da", "593ead094a9a203f00628baed789d186772434c0c39bc3ad530f157101c3e9435cced7e3466e707d11146b7ac2f7b75c3d71b1261b2c6841c6697491754ec609" },
                { "de", "e8e46d3c5496ce647f0ec1fcf0056c2029f512ed1468cd97c0967146026e9b86607f2a712e325b6dfaab33306eb7586b7d2572214291d05af46d3dbf0119e05b" },
                { "dsb", "e32fd2d2ad274fc633f97d8490faceae2c223169d7881817aeee7901709455e64b2feb23b5fef1712100fd3a09e2520ac2bd9acfddb4267d9b633a7ab25c6e37" },
                { "el", "0817315e7913dbcfab0a1615283a87d6cb7f48a1a845e81c0db0d01b05993f87eedd59fa6848903a21d77823a6134077cc9761c38101b5e2101d28b6835447ef" },
                { "en-CA", "07fdf425c427c850fe87badb1708bcce90500b131a663a28edf380e44b9403d2e020432de510d6835b684a3208a13f00d26c8ded60dc888cb4449e7a749e6e3e" },
                { "en-GB", "e059a7070745a54b92ebdc27d07698f5fef6293446cba2c677fa6ab18a801d770fe4f9f2085b8cf8e78337c8270a03e80863aa09a6f855c77b1769090227f700" },
                { "en-US", "96f9d53e1134cdd4951c1e3d4f5231054b618c45539a733634ebc43ee36c1a7416b6ee226d837a3be6c5081e2ec7c758487687e36771eb1165472dbcb33baf69" },
                { "eo", "03890ea578c874f29d384a9f056d8893a7772125fb1407a0884c1f81db87adaaee0fbae5729be91da025363eadd27d8f7f7ec8d31c3faa97dc1f74e097eeb8c7" },
                { "es-AR", "b6f94098f388d4bb413fe63148ced85d7357d5ef680bf95f97a903ec7e2b1189965df4e4709ac0589c1731965d65fe831ff673b41b5ad5381d56696df213fb1f" },
                { "es-CL", "9042d1541cf96d5f8282712e5bac64a4d947ef5dd82f41ee22ab410411ead9558f8676fef24e8b840c3e99a3f2d1493c13d19ca5a7f7b1ea13bd72c58d89652e" },
                { "es-ES", "f09c17d5643e5761cd8452d939cc3c07f7f8f37d1b43b2e8c6ab155ecd7b2e42ee2fff7dbd63ee2d58054488bfb3b26a11f6d3cc3cd45b9641d28596a5de267b" },
                { "es-MX", "bac2b7b98c5a24baac831824ef7f348a9544950c47509a9fb4e97d25ac2248187301eb9500518222c81bc09fd5ea19fe5cb0503d595f2301acdcc13eaaeeda8e" },
                { "et", "f4a10d762ca5175385c8b2e592357715bd9130d0db007c2da2823d707d4f42d1daaaac478d41b83127051fc2a3623d7e6fb87cedf1a772b452feba92a78d8ddc" },
                { "eu", "20c51d126f00467638bc233ec45a77c8283952ce8c5bcce096745d05e363ba0415d305094322d663e38ef4b059681813cbe76bb29c7d5530966ee17b063a9c02" },
                { "fa", "685429dc97ca477fc79120b928fb061a409e96e992b4f7c6a3641084f9e0e2cf6b2e110220c18172f21eeb9f8d2c73f15098de861a87a0675d36d6001daf6b5e" },
                { "ff", "624dc473ed84ebf59f31954d216202e570fe1e74bd65adceef2484d3c96ede0d1821145c7d7a11c48777e0a4083a06d9a1be87a1966c28043b124b101ce178d0" },
                { "fi", "2dc16d97fe612e1f24d45f3a633b011287cd40505dcae632d4a4aa985a709708f9f36fd43e1bb4cfc6d9efdd3b4e497a96708fc61628c75b01ac2127b0ce26e1" },
                { "fr", "3c3e92ac14e4bf97bae510ac1a75446d14b923c07ca241cd4c27399a0d507921aa51fcf84eec675b94aa7a817fe32debab770f8941cbc7d9aacad1c2e9c62d02" },
                { "fur", "7eaf34c31f6cf138785a150cf31f8d99e7379e3c95b3ee74c634a5534a5643782af0ae4407b3637726f5fd994ea381674981c9192a52f02177211c17b2d3d4c3" },
                { "fy-NL", "cb490f8cebc117cd8736f6511ca0f3b43875b29f1fd55b072e60d7ae04521169e4647607dcd94590b84c9ec271f00df136f891303dfcd044387a7a581ba2fa52" },
                { "ga-IE", "9ad68fd992643c31b31e8eaa7c7f1ec6b5844f7669722f290adba3b26ae111e851cafdec71e938e0b954f402255452b9fac474b53ad0cb27dc9dc41586d6bd38" },
                { "gd", "6e384f26a6e31fa14334ac7e13c74cf28ead1304258d0722a5c079c6e465a416b42f459151f8c8bb5a6853d66bb9a36cc958be2be7073dc9e9079608a504bd9e" },
                { "gl", "7853324806c35b9e62a1ddad162e7fe738945cd7964f24f6d9999059813737c60074baa74f52783af25b6f276d70bfbe5f7b4f489f26beaee867d45f7c5c0570" },
                { "gn", "ed15b0f9f86c9cf56479a98c9da96c5e8bda86acfb663561ce3a4b9bdd47cbd0fcd72855b860de1d900906ad8f406f5a8f0e02cbb22273f7209f57069810db6b" },
                { "gu-IN", "dc737efb82923001b3af03f9c4b5c24667a5982aa1042f8be05ac2f845882b69578f56f8cc805632c2e907dc60602a4fcb7bb0bbafa18cce6de06a4112899070" },
                { "he", "73dc7da248c232715cf4ab9bf0f2a6fe68fced64c8ba11b04ae417acea6255d0a33bb7043dbef113f96058a15d407c24dc2e5740defec8b61db6bb3487402004" },
                { "hi-IN", "60377b1b6baf601e22001395cf11ce5f804c91c11342c5d5bd37d9bc907fa660f00e499160b92f8054fd910f44ce0dd7c4506e812741c7bc209bc0fbc78c2ffb" },
                { "hr", "732a9d0c12146ef8a32d2fbe957afaf8546ffc18f643bddc287f9333be6dd2007b6491ca01096bff1c1d002e0d3938dae2821e54a2e70662b0b1bc7e717f8218" },
                { "hsb", "d45ab1be4283bc8c3f98a87bbbaf3386e1a2dea7f9848e27b4b72df3657402ef5203587390f804f8aeeacc04fa4a43c38a3c89bd4b63a97a625236192081e387" },
                { "hu", "c014446249304b39fbc2cd74ae0f16cfe8ed3aaac7494d6fc4911acc1ba7965eee9e099c9f1badd07b05bae2de9ae527cc55b854eb463811f89b48c991df5e1b" },
                { "hy-AM", "ffa0277cb0240a8759816cb69080330112f52380ef8d845730a553d6a7f4de6418d3514491364322979ee8b07b8deeae10a3a010a68cabcabc65d2ea124218c6" },
                { "ia", "9f5ca43d84bfd3a0e18d26818b960c97f3ecfba1436a9b1a411de380bfd7035158ed58065ee0e00e9f668bdb6ef9db56787839a385c9194fbfc69176cb1f8b6c" },
                { "id", "6be5210de263e70d18d0904da3076b3864559a24ca92012f9abcc239a11dbffdac2c7189729f57e075f2bbd60780917b99893dd69b7d2b11bab3f7d8a53bcbb7" },
                { "is", "f7be3e1ae120554b26768bb66c67bd39c61eb56e2edc64d1c8ca67cf9a3779484fbff8c79e4a0546cd8b5ad85687738e5f83c7ed0564d715062ebdfdb118945d" },
                { "it", "d2b6f3f44cffeeed09ac402125bd659724c5ead5d57815661bd68aed7714838507371bf5e6ef8339861fb1013f2ddc84c49103d47902ab4c59b22d131517931a" },
                { "ja", "0d0bdfa0337f0192ecc34ce32ff29a8ca1628f120f679ace02c6af35a0069d80cb0e1f43f04e0c8861117d84f5bf5d05b3ce2c15afb00507764cdae7e2b537d9" },
                { "ka", "5fe16ebbc32e9cb4a677f3d8dda84cc8062f24c3a2919dcc4d97f3dcda997ac4aa72f012fcad026fcc035b9ce6f2150e545ea8c6ea77607db062ec68e5d77baf" },
                { "kab", "ebf3d9b9e7895b1d751ffe9ec6ef029ef04b43f40b433a7955040a04dcf442e090b530ecd471cfa8e53950d22f14cd26203f78aecaf903c4f6e7eeb1d679e1a7" },
                { "kk", "423f4001b813072642c51dc0aa4165c2eae02a44a18084e58bc50b62d84bfab02481c5832fbee3fa61700d32246f088cd605d133eba0b27d89550d7b0fbe151b" },
                { "km", "ed441abbbaad552a32b9465d9972af4ff42a4775bd151410c7b873e2608de15c5a00b6050af5ee80a817ee98f44d5bf64b390fe856c56d354d344386097b0eb5" },
                { "kn", "11c7d6d17d4397cb50582f3051c93c1846ff3737ee695cef4aaf1d8cf08fbe627ced55676300292132fa72794c9f0f0ffa223fd9fc699abc3fb2d2dbb1e395d0" },
                { "ko", "df4180b184cf47351b2c8c08bb8eda8137d10381b52b83db626d9604e894d29fea4b0b6a8810a3bb952784035ecaf9185f11793a69c2b66412e088c4129600eb" },
                { "lij", "f70fa6df5a50595e31570cedb417ef4f66d27578e1f472481d1114a7acd6147988db5f41729a55f7c9b6d60baf12cd39810b2d1c89ab9786473ac36513e296a2" },
                { "lt", "69736d4ad9b8136e5a5313c7beb354a550141993d07320fd6945a6233d1861a536cf2395e11d147aa7aac071ce47f475de127ac1895223a042fc747a4fa4debb" },
                { "lv", "727a884aa0554c1e4502111d25d7f2bd8c05fab0960b029326429cdf80941ba956f06286a73cdbf0ce9192283a8b626fff2730fc1281594cc183929c29e39d29" },
                { "mk", "aec97d7b541b75ab3aeaab42c351855c9b4f427f08299937e162ac998357a0b55e50fd7d4cd591e0136c1103ac87d53711b576e664c111d2aeeb2f2e92e652f3" },
                { "mr", "f110692aec45cc451a2e6a98e6207f7e841a29818f1c5430e188e3ef7a0ef9d14b26a0abb81fff19018ee4603c4e5ea161c28b21af8c563ddc43421146385ab2" },
                { "ms", "e6b956ecac7d9a2f38ab95e1a3caa32ddeb4aefb4e7a6c397c0c33cc2ea9bfd086a90d0c6a37c6a2ef22e9f1f43fea5934f169d324af5bb5ccc56aed3d769478" },
                { "my", "29f599531764cf7173303e2e701cb9e4b055b51f43e72ccfa9567c91f75da737f3ff56943a6030fd4fc3936f60fe6858254a5b11271258f1af9b5e55f92b7861" },
                { "nb-NO", "5cfd646b8bb9684c57bc5a270c25bf84cab120f81f611a46976f65cafa49565b471c017ed14250671e1701de2cd439ee9fde0178b58d051ce09daf39fed3f33e" },
                { "ne-NP", "e81ecf7b6cc7116e72545c4c835ff231a9c47a31f8cb71c99c8f02d3e4b6174da5e487baf1a60884443bc149564ed1f26ce491612697898c835eac87e9f7c823" },
                { "nl", "a85a995d18cbd1914e44a3d5421a4d4fbd23f852a5d0e72f56468fa04dacf3c2bb7a394242cec42ba198c9e3a3f626d0fca99ec59216c48d98c0838e27bdd2e9" },
                { "nn-NO", "65fafa555b7b8e3fb887c44fc91f027cca75598dafefaa2f00c358cc82e2308c03b37ce5bac8a1fd0c71a8a8bbab74d4f586cfeca1d22867ad6bc3d196c90ad5" },
                { "oc", "68c2b13490fd69e36c005a4841f68183c4c9bb76ea5c65aae8a5eb4a9e8dcb5c6a78d6477af439a31d6a9555563042d86fc0bc7bca5fc224b6479850d5418bcf" },
                { "pa-IN", "1000e58baa51044bec6a2f570b4a0a88df5a8727cb0c13094de4e41ef4f012643f1d2faa95ea58c60d916a745526732c8787bdc2ebf87806cf0d66b2d3399c18" },
                { "pl", "9f52eacb7851f5cd3d83b560e10adc569475dbafc11cad4426bec7d68f7bb82a11a83e0efc1b571ae2b81be110cd3f07555901305be01fb933a8d2588e652c34" },
                { "pt-BR", "a2a148fa8dfa1407ebbabe8bef3a8232f4c9c7063403d478d30643a1a0e90745e5ba03f5653764cb085678cc997444e706de45a092b4b9ae6806028bfd6148e9" },
                { "pt-PT", "bd27848378480d8c82c02b50e640d6a5386c1428663502a587ed62a684c474394b0507c161570d5d03be8f754152582b64e47972eaf647428e6df2950a9cf033" },
                { "rm", "b40f498811a9ba1aaf1f56ba561fd584863c9a969a690a915cb6136bc48f8308a321127b3bd88a4adb0d0561f29b2c630a791b3f978fede156c3408b34ee0c73" },
                { "ro", "3826ea513af2339f3f802588de39a2c726b5f3d4b76cdff7438b718a221f8e8ad5f16c27105bdeaf70958f3999c0b9cc0004ed3d9e5fa7997f4a6fb7bf15b61b" },
                { "ru", "92ce290eacce03da91547fb8084b7db3571c54930a2c95b8bd41b6cfe82cd97ca684b7a1f1c84b5294f4b8d695fc5ac0e554ccc4b3089cddd1aec159a08e5066" },
                { "sc", "69f40cb81f2c673a7523d2b06dd990a87665b4461fc101eedee50aea41df1f5c971f1da32a8c350775aca39b815a4075aaec78c180ebb129eea37841321820d9" },
                { "sco", "0a54621ad6d3e65beb312ac33f5c082c3dc4d579823cd453868f81c9a11d73c55a636028e9217c4b2fd9e2d2cf15af0a5f40b9e8fc6fcf3c0e21206056d8b9d2" },
                { "si", "1dfd0fb98727e67d76cf65eace02536e95258b9cdec1f9a8537f2efa13dc6fc9c900f621cc46c10582e83af0eb30893887e34359264a448edccddcf820a89c46" },
                { "sk", "78ef31b9ece238f3cf84bb9aafa0c1dab3a1c01401722fe891cdead38191753358df257a851a63105c17bdc4d18f92a7761143a5ad958048e112a024d66d1625" },
                { "sl", "905e56e15563f9130819d69672429b7fba8fb617bf320f12cd7f889e66f0833130a55b0bc1150747dd8dc716830696c0cad9f689ec7a037448602332e2e29601" },
                { "son", "5cca1f19b5040d50fa35f40d5e677735306e8c503abaeac979ce7754337956dfaaf0b6d64a3dfef3d1dd7768b5b233c0028fdcd5b48e905bef93b6921ce4e8ba" },
                { "sq", "e7b0b518369b7dc70b5d5162df62f79132a12602bd2f1312f6a390dc4046541b57cd98e53ba53d0b2bf52e5f347d846d9a3ef2f0d823906b3855d26f991c1ca9" },
                { "sr", "5553c7aba0b7b7581ee6fb99a85a55054d369601545dd1b5c8f5d84c70d1725b4bc79d91c78e83eb88616806dbf4630d077b7d63f9512d52e5dcb02a3493ce7a" },
                { "sv-SE", "4e7da04ed696096e25b16bd9d6f71c276505cfc4bad348b9b90fb70d20ad6d8eea118fb4aedb045735367bc44ef190587d6c34f2e72a4e266313a46141608915" },
                { "szl", "72ddcbc69a9705d0e34bdfa2f29dd4c6dca9c0e598a5594f0c4f5f2496e84232cb912394894bb660a58a9cbebfe5e98ac920beddcdf6dee45401d3b1743b84ed" },
                { "ta", "643e4c5f8b6c6aa3b9c795bfa6371a74cc18433890a160ceb412a58182c420e3fcbb1c09eb035a9ab8d15d8c025fffa511f0861b05fe6c4fc2a1d23738d1001f" },
                { "te", "99dbb2d545a1409c27bf54ca313f8a16f6fa77a032fc2462e253b8b9ddb02762108c9750248d433c225ef558bcd5aea9e519dd4bb76dcd773a8c57e821af6528" },
                { "tg", "0aeb20935da5ffa2e73e2f6e50aefba3774a7fb5f5b1c060aae82b589ba965c6bec1b1d1e002fd48446647c2cd86324b84a6ee4ed49ad168441c8167b49bb378" },
                { "th", "908bad27f276452dc7eae2acf749e4b5f2077a5bcff4dc6689f4bedb5b6a05a0b3a035bb1b94114e161ecf877967bf4be5a68d376acafd5caa7db5f305adbeb6" },
                { "tl", "361d90205e902440512bcae4d479674ff6b2a044a2139722a0cf67f49fdf4ad500d80e003789c48bd5e60ea05a86e73727ee3b149ab1465fd3041100c19764f1" },
                { "tr", "8194022c875cb503d2c265ad54bb2af2289d14ec17ee2adda245532d46344d9dd6a03ab4341ab1ff497a4719dca143798d22df2f3ce8a806f8edfa1157462261" },
                { "trs", "e519e84a9aa3101ea1c8a9883dd7ed718637d27289fd6562a9cde8e2e29dfd6b9c9e266b362a957e21029a81c35fe8fc28d4e8bbc86f16f254e63147b54fbd3c" },
                { "uk", "63d216e4f5da794a80106f60f660842be413a469e8f5f527c15576347eae494e0a63b672df67e6dc7f6d40fb1e4f1fa97d0973910a540039513e9263639b16a6" },
                { "ur", "20f4144bed4b4dfc6287fd9cad86c9c0a62424ded0d596191d42f828451ee070851576ddae49150beabeb0077c01fe5e9dad1200c0a3c648f3950878474a3d1d" },
                { "uz", "ca2cde478f4f24951b9d1dbe7e0a36864e61112c9c77e4151932be96a1e8a81302db368abaf20337efe4c8d73b9fd127235a7892554e3b918659c6c316d79219" },
                { "vi", "0ebb872a9df1e15381d5436d9112b69e3d2eba23b2001f9061ad3724710b5271d9902bb940b163a780669ad62e82694b59cdac359c69752a91df0c80dc36d5f1" },
                { "xh", "d12c27ea0f4d0c33bbf1421ccb5cb4fc0fafc627a2aec28a91acc018f1c546a4a135812f10f2168833931e99060dbb2e676a3f122de6cb6f61736eb9b0a30440" },
                { "zh-CN", "90c059f39bc6bde65970c9ef93c1f4f3dc0e9109813718e0d342b5ceb46147dc93eafbc5c6dcaaf6501f4a0c0098fe361f2ca4f8888f63d5f78064ce9678d106" },
                { "zh-TW", "7494c82591c9a841a283ef6d2c6efdbdf2a5c30c5f19adf8a13a736f606c61ce8888a54f57b61c94b44236562b253cbdd97b1288953133e5782c765e17f40dee" }
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
